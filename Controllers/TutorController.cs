using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortal.Controllers
{
    [Authorize]
    public class TutorController : Controller
    {
        private readonly StudentPortalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutorController(StudentPortalDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Tutor/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existing = await _context.Tutors
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (existing != null) return RedirectToAction(nameof(Details));

            return View();
        }

        // POST: /Tutor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tutor model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            model.ApplicationUserId = user.Id;
            model.RegisteredAt = DateTime.UtcNow;
            model.HireDate = DateTime.UtcNow;
            model.IsActive = true;

            _context.Tutors.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details));
        }

        // GET: /Tutor/Details
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.ApplicationUser)
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Create));

            return View(tutor);
        }
    }
}
