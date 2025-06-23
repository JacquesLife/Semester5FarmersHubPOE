/// <summary>
/// Another long file responsible for managing the products in the application.
/// It was generated with CLI and is responsible for managing the products.
/// It has methods for creating, editing, deleting, and viewing products much like FarmerController.
/// It also has methods for filtering products by date.
/// It uses the ApplicationDbContext to interact with the database.
/// It uses the IFileService to handle file uploads.
/// It uses the UserManager to get the current user and their roles.
/// It uses the ILogger to log information and errors.
/// It uses the Authorize attribute to restrict access to certain actions.
/// </summary>
/// <reference = https://learn.microsoft.com/en-us/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator?view=aspnetcore-9.0


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Programming3A.Database;
using Programming3A.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Programming3A.Services;

namespace Programming3A.Controllers
{
    [Authorize] // Require authentication
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ProductController> _logger;
        private readonly IFileService _fileService;

        public ProductController(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager,
            ILogger<ProductController> logger,
            IFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _fileService = fileService;
        }

        /// <summary>
        /// Retrieves a list of products from the database.
        /// If the user is a farmer, only their products are shown.
        /// The products can be filtered by production date.
        /// </summary>

        // GET: Product
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            // Log the date filter values
            _logger.LogInformation("Retrieving products with date filter - From: {FromDate}, To: {ToDate}", 
                fromDate?.ToString("yyyy-MM-dd") ?? "any", 
                toDate?.ToString("yyyy-MM-dd") ?? "any");

            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            var query = _context.Products
                .Include(p => p.Farmer)
                .AsNoTracking();

            // If user is an admin or employee, show all products
            if (roles.Contains("Employee") || roles.Contains("Admin"))
            {
                // No filter needed, show all products
            }
            // If user is a farmer, only show their products
            else
            {
                var farmer = await _context.Farmers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.UserId == user.Id);

                if (farmer == null)
                {
                    _logger.LogWarning("No farmer profile found for user {UserId}", user.Id);
                    return View(new List<ProductModel>());
                }

                query = query.Where(p => p.FarmerId == farmer.FarmerId);
            }

            if (fromDate.HasValue)
            {
                // Ensure the date is set to the start of the day
                query = query.Where(p => p.ProductionDate >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                // Ensure the date is set to the end of the day
                query = query.Where(p => p.ProductionDate <= toDate.Value.Date);
            }

            var products = await query.OrderByDescending(p => p.ProductionDate).ToListAsync();
            
            // Store the filter values in ViewData to maintain them in the view
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");

            return View(products);
        }

        /// <summary>
        /// Retrieves the details of a specific product.
        /// If the product is not found, a 404 error is returned.
        /// </summary>

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the product with its farmer information
            var productModel = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        /// <summary>
        /// Creates a new product.
        /// Checks if the user has a farmer profile before allowing them to create a product.
        /// If the user does not have a farmer profile, they are redirected to the home page.
        /// </summary>

        // GET: Product/Create
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create()
        {
            // Check if the user has a farmer profile
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _context.Farmers
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null)
            {
                // Log the warning and redirect to home
                _logger.LogWarning("User without farmer profile attempted to access Create Product page");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        /// <summary>
        /// Creates a new product.
        /// Validates the model state and checks if the user has a farmer profile.
        /// If the user does not have a farmer profile, they are redirected to the home page.
        /// Handles image upload and saves the product to the database.
        /// If the product is created successfully, the user is redirected to the index page.
        /// If there is an error, the user is shown the create product page with the error message.
        /// </summary>

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create([Bind("ProductName,Category,Description,ImageFile,Price,ProductionDate")] ProductModel productModel)
        {
            _logger.LogInformation("Starting product creation process...");

            // Remove FarmerId validation since we'll set it ourselves
            ModelState.Remove("FarmerId");
            ModelState.Remove("Farmer");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", 
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                return View(productModel);
            }

            try
            {
                // Get current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("Unable to get current user. User.Identity.Name: {Name}", User.Identity?.Name);
                    ModelState.AddModelError("", "Unable to create product. Please try logging in again.");
                    return View(productModel);
                }

                // Get the farmer profile for the current user
                var farmer = await _context.Farmers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.UserId == user.Id);

                if (farmer == null)
                {
                    _logger.LogWarning("User without farmer profile attempted to create a product");
                    return RedirectToAction("Index", "Home");
                }

                // Set the FarmerId
                productModel.FarmerId = farmer.FarmerId;

                // Handle image upload
                if (productModel.ImageFile != null)
                {
                    productModel.ImagePath = await _fileService.SaveFileAsync(productModel.ImageFile, "uploads/products");
                }

                // Set the production date to the current date if not provided
                _logger.LogInformation("Creating product for farmer {FarmerId}. Product details: {@ProductModel}", 
                    farmer.FarmerId, 
                    new { productModel.ProductName, productModel.Category, productModel.Price });

                _context.Add(productModel);
                
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Product created successfully. ProductId: {ProductId}, Changes: {Changes}", 
                    productModel.ProductId, changes);

                return RedirectToAction(nameof(Index));
            }
            // Handle exceptions
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An error occurred while creating product");
                ModelState.AddModelError("", "An error occurred while creating the product. Please try again.");
                return View(productModel);
            }
        }

        /// <summary>
        /// Retrieves the edit page for a specific product.
        /// Checks if the user has permission to edit the product.
        /// If the user does not have permission, they are shown a forbidden error.
        /// If the product is not found, a 404 error is returned.
        /// </summary>

        // GET: Product/Edit/5
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isFarmer = await _userManager.IsInRoleAsync(user, "Farmer");

            // Only allow farmers to edit their own products, but admins can edit any
            if (!isAdmin && (!isFarmer || productModel.FarmerId != (await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == user.Id))?.FarmerId))
            {
                return Forbid();
            }

            return View(productModel);
        }

        /// <summary>
        /// Edits an existing product.
        /// Validates the model state and checks if the user has permission to edit the product.
        /// If the user does not have permission, they are shown a forbidden error.
        /// Handles image upload and updates the product in the database.
        /// If the product is updated successfully, the user is redirected to the index page.
        /// If there is an error, the user is shown the edit product page with the error message.
        /// </summary>

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,FarmerId,ProductName,Category,Description,ImageFile,Price,ProductionDate")] ProductModel productModel)
        {
            if (id != productModel.ProductId)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isFarmer = await _userManager.IsInRoleAsync(user, "Farmer");

            // Only allow farmers to edit their own products, but admins can edit any
            if (!isAdmin && (!isFarmer || productModel.FarmerId != (await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == user.Id))?.FarmerId))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload if a new image is provided
                    if (productModel.ImageFile != null)
                    {
                        productModel.ImagePath = await _fileService.SaveFileAsync(productModel.ImageFile, "uploads/products");
                    }
                    else
                    {
                        // Preserve the existing image path
                        var existingProduct = await _context.Products
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.ProductId == id);
                        productModel.ImagePath = existingProduct?.ImagePath;
                    }

                    _context.Update(productModel);
                    await _context.SaveChangesAsync();
                }
                // Handle concurrency exception
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductModelExists(productModel.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productModel);
        }

        /// <summary>
        /// Deletes a specific product.
        /// Checks if the user has permission to delete the product.
        /// If the user does not have permission, they are shown a forbidden error.
        /// If the product is not found, a 404 error is returned.
        /// </summary>

        // GET: Product/Delete/5
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products.Include(p => p.Farmer).FirstOrDefaultAsync(m => m.ProductId == id);
            if (productModel == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isFarmer = await _userManager.IsInRoleAsync(user, "Farmer");

            // Only allow farmers to delete their own products, but admins can delete any
            if (!isAdmin && (!isFarmer || productModel.FarmerId != (await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == user.Id))?.FarmerId))
            {
                return Forbid();
            }

            return View(productModel);
        }

        /// <summary>
        /// Deletes a specific product.
        /// Checks if the user has permission to delete the product.
        /// If the user does not have permission, they are shown a forbidden error.
        /// Handles image deletion and removes the product from the database.
        /// If the product is deleted successfully, the user is redirected to the index page.
        /// If there is an error, the user is shown the delete product page with the error message.
        /// </summary>

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productModel = await _context.Products.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isFarmer = await _userManager.IsInRoleAsync(user, "Farmer");

            // Only allow farmers to delete their own products, but admins can delete any
            if (!isAdmin && (!isFarmer || productModel.FarmerId != (await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == user.Id))?.FarmerId))
            {
                return Forbid();
            }

            // Delete the product's image file if it exists
            if (!string.IsNullOrEmpty(productModel.ImagePath))
            {
                _fileService.DeleteFile(productModel.ImagePath);
            }

            _context.Products.Remove(productModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductModelExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}



// ---------------------------------End of File-----------------------------------------------------