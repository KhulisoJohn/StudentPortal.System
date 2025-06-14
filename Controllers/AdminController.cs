using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Take(50).ToListAsync();
            var model = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles
                .Select(r => r.Name ?? string.Empty)
                .ToListAsync();

            return View(new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                SelectedRoles = userRoles.ToList(),
                AllRoles = allRoles
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllRoles = await _roleManager.Roles
                    .Select(r => r.Name ?? string.Empty)
                    .ToListAsync();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            // Validate each role
            foreach (var role in model.SelectedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    ModelState.AddModelError("", $"Role '{role}' does not exist.");
                    model.AllRoles = await _roleManager.Roles
                        .Select(r => r.Name ?? string.Empty)
                        .ToListAsync();
                    return View(model);
                }
            }

            // Update user info
            user.FullName = model.FullName;
            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                model.AllRoles = await _roleManager.Roles
                    .Select(r => r.Name ?? string.Empty)
                    .ToListAsync();
                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var toRemove = currentRoles.Except(model.SelectedRoles).ToList();
            var toAdd = model.SelectedRoles.Except(currentRoles).ToList();

            if (toRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Error removing roles.");
                    return View(model);
                }
            }

            if (toAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, toAdd);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Error adding roles.");
                    return View(model);
                }
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return View(new AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errorModel = new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                };
                ModelState.AddModelError("", "Failed to delete user.");
                return View("Delete", errorModel);
            }

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> MakeMeAdmin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found.");

            // TODO: Replace with config-based check
            var allowedEmails = new[] { "admin@domain.com" };
            if (!allowedEmails.Contains(user.Email))
                return Forbid("You are not allowed to promote yourself.");

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var createRole = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!createRole.Succeeded)
                    return StatusCode(500, "Failed to create Admin role.");
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var addRole = await _userManager.AddToRoleAsync(user, "Admin");
                if (!addRole.Succeeded)
                    return StatusCode(500, "Failed to add Admin role.");

                user.Role = "Admin";
                await _userManager.UpdateAsync(user);
            }

            TempData["Success"] = "You are now an Admin!";
            return RedirectToAction(nameof(Index));
        }
    }
}
