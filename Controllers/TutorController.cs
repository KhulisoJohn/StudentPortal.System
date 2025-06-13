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
    [Authorize(Roles = "Tutor")]
    public class TutorController : Controller
    {
        private readonly StudentPortalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutorController(StudentPortalDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existing = await _context.Tutors
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (existing != null) return RedirectToAction(nameof(Details));

            ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tutor model, int[] selectedSubjects)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existingTutor = await _context.Tutors
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);
            if (existingTutor != null)
            {
                ModelState.AddModelError("", "You already have a tutor profile.");
                return RedirectToAction(nameof(Details));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            if (selectedSubjects == null || selectedSubjects.Length == 0)
            {
                ModelState.AddModelError("", "You must select at least one subject you can tutor.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            var validSubjectIds = await _context.Subjects.Select(s => s.Id).ToListAsync();
            if (!selectedSubjects.All(id => validSubjectIds.Contains(id)))
            {
                ModelState.AddModelError("", "One or more selected subjects are invalid.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            model.ApplicationUserId = user.Id;
            model.RegisteredAt = DateTime.UtcNow;
            model.HireDate = DateTime.UtcNow;
            model.IsActive = true;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Tutors.Add(model);

                var tutorSubjects = selectedSubjects.Select(subjectId => new TutorSubject
                {
                    Tutor = model,
                    SubjectId = subjectId,
                    Approved = false
                }).ToList();

                _context.TutorSubjects.AddRange(tutorSubjects);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Tutor profile created successfully! Waiting for subject approval.";
                return RedirectToAction(nameof(Details));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "An error occurred while creating your profile. Please try again.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                // TODO: Log the exception 'ex' here using your logging framework
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.ApplicationUser)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Create));

            return View(tutor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinChannel(int subjectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Create));

            var isApprovedForSubject = tutor.TutorSubjects
                .Any(ts => ts.SubjectId == subjectId && ts.Approved);

            if (!isApprovedForSubject)
            {
                TempData["ErrorMessage"] = "You are not approved to tutor this subject.";
                return RedirectToAction(nameof(Details));
            }

            TempData["SuccessMessage"] = "Successfully joined the subject channel!";
            return RedirectToAction(nameof(Details));
        }

        [HttpGet]
        public async Task<IActionResult> UploadMaterial(int subjectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Create));

            var isApprovedForSubject = tutor.TutorSubjects
                .Any(ts => ts.SubjectId == subjectId && ts.Approved);

            if (!isApprovedForSubject)
            {
                TempData["ErrorMessage"] = "You are not approved to upload materials for this subject.";
                return RedirectToAction(nameof(Details));
            }

            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
            {
                TempData["ErrorMessage"] = "Subject not found.";
                return RedirectToAction(nameof(Details));
            }

            ViewBag.SubjectName = subject.Name;
            return View(new TutorMaterial { SubjectId = subjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMaterial(TutorMaterial model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Create));

            var isApprovedForSubject = await _context.TutorSubjects
                .AnyAsync(ts => ts.TutorId == tutor.Id &&
                               ts.SubjectId == model.SubjectId &&
                               ts.Approved);

            if (!isApprovedForSubject)
            {
                TempData["ErrorMessage"] = "You are not approved to upload materials for this subject.";
                return RedirectToAction(nameof(Details));
            }

            if (!ModelState.IsValid)
            {
                var subject = await _context.Subjects.FindAsync(model.SubjectId);
                ViewBag.SubjectName = subject?.Name ?? "Unknown Subject";
                return View(model);
            }

            // TODO: Handle file uploads here if TutorMaterial contains files (save file, set path etc.)

            model.UploadDate = DateTime.UtcNow;
            model.TutorId = tutor.Id;

            _context.TutorMaterials.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Material uploaded successfully!";
            return RedirectToAction(nameof(Details));
        }
    }
}
