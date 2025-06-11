using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;
using System;
using System.Threading.Tasks;

namespace StudentPortal.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly StudentPortalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(StudentPortalDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Student/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existingStudent = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (existingStudent != null)
                return RedirectToAction(nameof(Details));

            return View();
        }

        // POST: /Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            // Double-check to prevent duplicate student record for the user
            var existingStudent = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (existingStudent != null)
                return RedirectToAction(nameof(Details));

            model.ApplicationUserId = user.Id;
            model.EnrollmentDate = DateTime.UtcNow;

            _context.Students.Add(model);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Log the exception (not shown here, add your logger)
                ModelState.AddModelError("", "An error occurred while saving your data. Please try again.");
                return View(model);
            }

            return RedirectToAction(nameof(Details));
        }

        // GET: /Student/Details
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .AsNoTracking()
                .Include(s => s.ApplicationUser)
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
                return RedirectToAction(nameof(Create));

            return View(student);
        }
    }
}
