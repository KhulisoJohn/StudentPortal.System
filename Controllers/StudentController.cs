using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Data;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System;
using System.Linq;
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

            var existing = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (existing != null) return RedirectToAction(nameof(Details));

            return View();
        }

        // POST: /Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            model.UserId = user.Id;
            model.EnrollmentDate = DateTime.UtcNow;

            _context.Students.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details));
        }

        // GET: /Student/Details
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null) return RedirectToAction(nameof(Create));

            return View(student);
        }
    }
}
