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

