using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortalSystem.Models;
using StudentPortalSystem.Data;
using StudentPortalSystem.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace StudentPortalSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StudentPortalDbContext _context;

        private readonly string[] _allowedRoles = new[] { "Student", "Tutor" };

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

        private async Task EnsureRolesExistAsync()
        {
            foreach (var role in _allowedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
    
            {
        
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email!);
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
                FullName = model.FullName!,
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            if (model.Role == "Student")
            {
                var student = new Student
                {
                    ApplicationUserId = user.Id,
                    EnrollmentDate = DateTime.UtcNow
                };
                _context.Students.Add(student);
            }
            else if (model.Role == "Tutor")
            {
                var tutor = new Tutor
                {
                    ApplicationUserId = user.Id,
                    HireDate = DateTime.UtcNow
                };
                _context.Tutors.Add(tutor);
            }

            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Create", "Students");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, model.Password!, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "Admin");

                if (ProfileNeedsCompletion(user))
                    return RedirectToAction("Index",roles.Contains("Student") ? "Student" : "Tutor");

                return RedirectToAction("Index", roles.Contains("Student") ? "Student" : "Tutor");
            }

            if (result.IsLockedOut)
                ModelState.AddModelError("", "Your account has been locked. Try again later.");
            else
                ModelState.AddModelError("", "Invalid login attempt.");

            return View(model);
        }

        private bool ProfileNeedsCompletion(ApplicationUser user)
        {
            return string.IsNullOrEmpty(user.PhoneNumber) ||
                   (user.Student == null && user.Tutor == null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var profileVm = new ProfileViewModel
            {
                Email = user.Email!,
                FullName = user.FullName!,
                CreatedAt = user.CreatedAt,
                IsStudent = user.Student != null,
                IsTutor = user.Tutor != null,
                PhoneNumber = user.PhoneNumber
            };

            return View(profileVm);
        }
    }
}
