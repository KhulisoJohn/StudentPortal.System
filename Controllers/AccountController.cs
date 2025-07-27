using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortalSystem.Enums;
using StudentPortalSystem.Data;
using StudentPortalSystem.Models;
using StudentPortalSystem.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StudentPortalDbContext _context;

        private readonly UserRole[] _allowedRoles = new[] { UserRole.Student, UserRole.Tutor };

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            StudentPortalDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Ensure required roles exist in DB
        private async Task EnsureRolesExistAsync()
        {
            foreach (var role in _allowedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(model);
            }

            await EnsureRolesExistAsync();

            if (!_allowedRoles.Contains(model.Role))
            {
                ModelState.AddModelError("Role", "Invalid role selected.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = model.PhoneNumber
            };

            var createUserResult = await _userManager.CreateAsync(user, model.Password);
            if (!createUserResult.Succeeded)
            {
                foreach (var error in createUserResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            // Assign user role
            var addToRoleResult = await _userManager.AddToRoleAsync(user, model.Role.ToString());
            if (!addToRoleResult.Succeeded)
            {
                // Rollback user creation if role assignment fails
                await _userManager.DeleteAsync(user);
                ModelState.AddModelError("", "Failed to assign role. Please try again.");
                return View(model);
            }

            // Create related entities based on role
            switch (model.Role)
            {
                case UserRole.Tutor:
                    var tutor = new Tutor
                    {
                        ApplicationUserId = user.Id,
                        HireDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.Tutors.Add(tutor);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Tutor");

                case UserRole.Student:
                    // Optionally create minimal Student entity here to avoid enrollment step later
                    // Or redirect to enrollment/details page
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Details", "Student");

                default:
                    // Should never reach here due to earlier validation
                    return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                if (roles.Contains(UserRole.Admin.ToString()))
                    return RedirectToAction("Index", "Admin");

                if (roles.Contains(UserRole.Student.ToString()))
                    return RedirectToAction("Index", "Student");

                if (roles.Contains(UserRole.Tutor.ToString()))
                    return RedirectToAction("Index", "Tutor");

                // Default fallback
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
                ModelState.AddModelError("", "Your account has been locked. Try again later.");
            else
                ModelState.AddModelError("", "Invalid login attempt.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
