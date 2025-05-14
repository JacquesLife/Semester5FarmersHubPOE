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

        // GET: Product
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            _logger.LogInformation("Retrieving products with date filter - From: {FromDate}, To: {ToDate}", 
                fromDate?.ToString("yyyy-MM-dd") ?? "any", 
                toDate?.ToString("yyyy-MM-dd") ?? "any");

            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            var query = _context.Products
                .Include(p => p.Farmer)
                .AsNoTracking();

            // If user is a farmer, only show their products
            if (!roles.Contains("Employee"))
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
                query = query.Where(p => p.ProductionDate >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate <= toDate.Value.Date);
            }

            var products = await query.OrderByDescending(p => p.ProductionDate).ToListAsync();
            
            // Store the filter values in ViewData to maintain them in the view
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

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
                _logger.LogWarning("User without farmer profile attempted to access Create Product page");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

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

                _logger.LogInformation("Creating product for farmer {FarmerId}. Product details: {@ProductModel}", 
                    farmer.FarmerId, 
                    new { productModel.ProductName, productModel.Category, productModel.Price });

                _context.Add(productModel);
                
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Product created successfully. ProductId: {ProductId}, Changes: {Changes}", 
                    productModel.ProductId, changes);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product");
                ModelState.AddModelError("", "An error occurred while creating the product. Please try again.");
                return View(productModel);
            }
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (productModel == null)
            {
                return NotFound();
            }

            // Check if user has permission to edit this product
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _context.Farmers
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null || farmer.FarmerId != productModel.FarmerId)
            {
                _logger.LogWarning("Unauthorized attempt to edit product {ProductId} by user {UserId}", id, user.Id);
                return Forbid();
            }
            
            return View(productModel);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,FarmerId,ProductName,Category,Description,ImageFile,Price,ProductionDate")] ProductModel productModel)
        {
            if (id != productModel.ProductId)
            {
                return NotFound();
            }

            // Check if user has permission to edit this product
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _context.Farmers
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null || farmer.FarmerId != productModel.FarmerId)
            {
                _logger.LogWarning("Unauthorized attempt to edit product {ProductId} by user {UserId}", id, user.Id);
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

        // GET: Product/Delete/5
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (productModel == null)
            {
                return NotFound();
            }

            // Check if user has permission to delete this product
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _context.Farmers
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null || farmer.FarmerId != productModel.FarmerId)
            {
                _logger.LogWarning("Unauthorized attempt to delete product {ProductId} by user {UserId}", id, user.Id);
                return Forbid();
            }

            return View(productModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productModel = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (productModel == null)
            {
                return NotFound();
            }

            // Check if user has permission to delete this product
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _context.Farmers
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null || farmer.FarmerId != productModel.FarmerId)
            {
                _logger.LogWarning("Unauthorized attempt to delete product {ProductId} by user {UserId}", id, user.Id);
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
