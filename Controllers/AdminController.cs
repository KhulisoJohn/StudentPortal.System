using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalSystem.Enums;
using StudentPortalSystem.Models;
using StudentPortalSystem.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // --- List first 50 users ---
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Take(50).ToListAsync();
            var model = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                model.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Role = user.Role
                });
            }

            return View(model);
        }

        // --- Edit user - GET ---
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                SelectedRole = user.Role,
                AllRoles = System.Enum.GetNames(typeof(UserRole)).ToList()
            };

            return View(model);
        }

        // --- Edit user - POST ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllRoles = System.Enum.GetNames(typeof(UserRole)).ToList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // Update user info
            user.FullName = model.FullName;
            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
            }
            user.PhoneNumber = model.PhoneNumber;
            user.Role = (UserRole)model.SelectedRole;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                model.AllRoles = System.Enum.GetNames(typeof(UserRole)).ToList();
                return View(model);
            }

            // Role update logic
            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoleName = model.SelectedRole.ToString();

            if (!currentRoles.Contains(selectedRoleName))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, selectedRoleName);
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // --- Delete user - GET ---
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(new AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Role = user.Role
            });
        }

        // --- Delete user - POST ---
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var model = new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Role = user.Role
                };
                ModelState.AddModelError("", "Failed to delete user.");
                return View("Delete", model);
            }

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // --- Promote to Admin ---
        [Authorize]
        public async Task<IActionResult> MakeMeAdmin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found.");

            var allowedEmails = new[] { "admin@domain.com" };
            if (!allowedEmails.Contains(user.Email))
                return Forbid("You are not allowed to promote yourself.");

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var createResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!createResult.Succeeded)
                    return StatusCode(500, "Failed to create Admin role.");
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var addResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!addResult.Succeeded)
                    return StatusCode(500, "Failed to add Admin role.");

                user.Role = UserRole.Admin;
                await _userManager.UpdateAsync(user);
            }

            TempData["Success"] = "You are now an Admin!";
            return RedirectToAction(nameof(Index));
        }
    }
}
