using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Programming3A.Database;
using Programming3A.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Programming3A.Controllers
{
    [Authorize] // Base authorization
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FarmerController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FarmerController(
            ApplicationDbContext context,
            ILogger<FarmerController> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            
            // Ensure roles exist
            EnsureRolesExistAsync().Wait();
        }

        private async Task EnsureRolesExistAsync()
        {
            string[] roles = { "Employee", "Farmer" };
            
            foreach (var roleName in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    _logger.LogInformation("Creating role: {RoleName}", roleName);
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // GET: Farmer
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            // If user is an employee, show all farmers
            if (roles.Contains("Employee"))
            {
                _logger.LogInformation("Employee viewing all farmers");
                var allFarmers = await _context.Farmers
                    .Include(f => f.User)
                    .AsNoTracking()
                    .ToListAsync();
                return View(allFarmers);
            }
            
            // If user is a farmer, show only their profile
            _logger.LogInformation("Farmer viewing their own profile");
            var farmer = await _context.Farmers
                .Include(f => f.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            return View(farmer != null ? new List<FarmerModel> { farmer } : new List<FarmerModel>());
        }

        // GET: Farmer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details called with null ID.");
                return NotFound();
            }

            var farmerModel = await _context.Farmers
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FarmerId == id);

            if (farmerModel == null)
            {
                _logger.LogWarning("Farmer with ID {Id} not found.", id);
                return NotFound();
            }

            return View(farmerModel);
        }

        // GET: Farmer/Create
        [Authorize(Roles = "Employee")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Farmer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create([Bind("FullName,ContactNumber")] FarmerModel farmerModel)
        {
            _logger.LogInformation("Starting farmer creation process...");

            // Remove validation errors for UserId and User since we'll set these ourselves
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", 
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                return View(farmerModel);
            }

            try
            {
                // Create a new user account for the farmer with simplified credentials for demo purposes
                var firstName = farmerModel.FullName.Split(' ')[0].ToLower(); 
                var username = $"{firstName}@farmerhub";
                var password = "Farmer123!"; 

                var user = new IdentityUser
                {
                    UserName = username,
                    Email = username,
                    PhoneNumber = farmerModel.ContactNumber
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create user account. Errors: {Errors}", 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    ModelState.AddModelError("", "Failed to create user account.");
                    return View(farmerModel);
                }

                // Assign the Farmer role
                await _userManager.AddToRoleAsync(user, "Farmer");

                // Create the farmer profile
                farmerModel.UserId = user.Id;
                _context.Add(farmerModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Farmer created successfully. ID: {Id}, Username: {Username}", 
                    farmerModel.FarmerId, username);

                // Store the credentials in TempData to show them on the next page
                TempData["FarmerCredentials"] = $"Username: {username}\nPassword: {password}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new farmer.");
                ModelState.AddModelError("", "An error occurred while creating the farmer profile. Please try again.");
                return View(farmerModel);
            }
        }

        // GET: Farmer/Edit/5
        [Authorize(Roles = "Employee,Farmer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("User not found when attempting to edit farmer");
                return Challenge();
            }

            var isEmployee = await _userManager.IsInRoleAsync(currentUser, "Employee");
            var isFarmer = await _userManager.IsInRoleAsync(currentUser, "Farmer");

            var farmerModel = await _context.Farmers
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FarmerId == id);

            if (farmerModel == null)
            {
                return NotFound();
            }

            // If user is a farmer, they can only edit their own profile
            if (isFarmer && !isEmployee && farmerModel.UserId != currentUser.Id)
            {
                _logger.LogWarning("Farmer attempted to access edit page for another farmer's profile");
                return Forbid();
            }

            return View(farmerModel);
        }

        // POST: Farmer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee,Farmer")]
        public async Task<IActionResult> Edit(int id, [Bind("FarmerId,UserId,FullName,ContactNumber")] FarmerModel farmerModel)
        {
            _logger.LogInformation(
                "Edit action started - ID: {Id}, FarmerId: {FarmerId}, FullName: {FullName}, ContactNumber: {ContactNumber}", 
                id, farmerModel.FarmerId, farmerModel.FullName, farmerModel.ContactNumber);

            // Check if user is authorized to edit this farmer
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("User not found when attempting to edit farmer");
                return Challenge();
            }

            var isEmployee = await _userManager.IsInRoleAsync(currentUser, "Employee");
            var isFarmer = await _userManager.IsInRoleAsync(currentUser, "Farmer");

            // If user is a farmer, they can only edit their own profile
            if (isFarmer && !isEmployee)
            {
                var farmerProfile = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                
                if (farmerProfile == null || farmerProfile.FarmerId != id)
                {
                    _logger.LogWarning("Farmer attempted to edit another farmer's profile");
                    return Forbid();
                }
            }
            else if (!isEmployee)
            {
                _logger.LogWarning("Unauthorized user attempted to edit farmer");
                return Forbid();
            }

            if (id != farmerModel.FarmerId)
            {
                _logger.LogWarning("ID mismatch in Edit action. URL ID: {UrlId}, Model ID: {ModelId}", id, farmerModel.FarmerId);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmerModel);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Farmer updated successfully - ID: {Id}", id);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerModelExists(farmerModel.FarmerId))
                    {
                        _logger.LogWarning("Farmer not found during update - ID: {Id}", id);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", 
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
            }

            return View(farmerModel);
        }

        // GET: Farmer/Delete/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            // First check if user is actually an employee
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found when attempting to delete farmer");
                return Challenge();
            }

            var isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
            if (!isEmployee)
            {
                _logger.LogWarning("Non-employee user attempted to delete farmer");
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var farmerModel = await _context.Farmers
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FarmerId == id);
            if (farmerModel == null)
            {
                return NotFound();
            }

            return View(farmerModel);
        }

        // POST: Farmer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // First check if user is actually an employee
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found when attempting to delete farmer");
                return Challenge();
            }

            var isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
            if (!isEmployee)
            {
                _logger.LogWarning("Non-employee user attempted to delete farmer");
                return Forbid();
            }

            try
            {
                var farmerModel = await _context.Farmers
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.FarmerId == id);

                if (farmerModel == null)
                {
                    _logger.LogWarning("Farmer with ID {Id} not found during deletion.", id);
                    return NotFound();
                }

                // Start a transaction to ensure both operations succeed or fail together
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Delete associated products first
                    var products = await _context.Products
                        .Where(p => p.FarmerId == id)
                        .ToListAsync();
                    
                    if (products.Any())
                    {
                        _context.Products.RemoveRange(products);
                        await _context.SaveChangesAsync();
                    }

                    // Delete the farmer first
                    _context.Farmers.Remove(farmerModel);
                    await _context.SaveChangesAsync();

                    // Delete the associated user account last
                    var identityUser = await _userManager.FindByIdAsync(farmerModel.UserId);
                    if (identityUser != null)
                    {
                        var result = await _userManager.DeleteAsync(identityUser);
                        if (!result.Succeeded)
                        {
                            throw new ApplicationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();
                    
                    _logger.LogInformation("Successfully deleted farmer with ID {Id}", id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occurred while deleting farmer with ID {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete farmer with ID {Id}", id);
                TempData["ErrorMessage"] = "Failed to delete the farmer. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool FarmerModelExists(int id)
        {
            return _context.Farmers.Any(e => e.FarmerId == id);
        }
    }
}