/// <summary>
/// The admin controller handles user management tasks such as assigning roles.
/// It is restricted to users with the Admin role. the admin is seeded in the DataSeeder class in the Seed folder.
/// </summary>
/// <reference = https://www.youtube.com/watch?app=desktop&v=Y6DCP-yH-9Q 

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Programming3A.Constants;
using Programming3A.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Programming3A.Controllers;


    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// This method retrieves all users from the database and their roles.
        /// It creates a list of UserRoleModel objects to display the users and their roles in the view.
        /// </summary>
        /// <returns></returns>

        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserRoleModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserRoleModel
            {
                UserId = user.Id,
                Email = user.Email,
                CurrentRole = roles.FirstOrDefault() ?? "None"
            }); }

            return View(model);
        }         

        /// <summary>
        /// This method assigns a role to a user.
        /// It first checks if the user exists and if they are not already in the specified role.
        /// If they are not, it removes any previous roles and adds the new role.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null && !await _userManager.IsInRoleAsync(user, role))
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles); // remove previous roles to avoid duplicates
                await _userManager.AddToRoleAsync(user, role);
            }

            return RedirectToAction("ManageUsers");
        }
    }


//---------------------------------------------------End of File-----------------------------------------------------