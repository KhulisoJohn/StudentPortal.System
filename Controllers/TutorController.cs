using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalSystem.Chat;
using StudentPortalSystem.Data;
using StudentPortalSystem.Models;


namespace StudentPortalSystem.Controllers
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

        // GET: Tutor/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existing = await _context.Tutors.FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);
            if (existing != null) return RedirectToAction(nameof(Details));

            ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
            return View();
        }

        // POST: Tutor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tutor model, int[] selectedSubjects)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existingTutor = await _context.Tutors.FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);
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
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "An error occurred while creating your profile. Please try again.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }
        }

        // GET: Tutor/Details
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

            if (tutor == null)
            {
                // Auto-create tutor if not exists
                tutor = new Tutor
                {
                    ApplicationUserId = user.Id,
                    RegisteredAt = DateTime.UtcNow,
                    HireDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Tutors.Add(tutor);
                await _context.SaveChangesAsync();
            }

            return View(tutor);
        }

        // GET: Tutor/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Details));

            ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
            ViewBag.SelectedSubjects = tutor.TutorSubjects.Select(ts => ts.SubjectId).ToArray();

            return View(tutor);
        }

        // POST: Tutor/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tutor model, int[] selectedSubjects)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.Id == model.Id && t.ApplicationUserId == user.Id);

            if (tutor == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            if (selectedSubjects == null || selectedSubjects.Length == 0)
            {
                ModelState.AddModelError("", "You must select at least one subject.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            var validSubjectIds = await _context.Subjects.Select(s => s.Id).ToListAsync();
            if (!selectedSubjects.All(id => validSubjectIds.Contains(id)))
            {
                ModelState.AddModelError("", "One or more selected subjects are invalid.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            tutor.IsActive = model.IsActive;
            tutor.HireDate = model.HireDate;

            _context.TutorSubjects.RemoveRange(tutor.TutorSubjects);

            var newSubjects = selectedSubjects.Select(subjectId => new TutorSubject
            {
                TutorId = tutor.Id,
                SubjectId = subjectId,
                Approved = false // Reset approval on update - or adjust logic as you see fit
            }).ToList();

            _context.TutorSubjects.AddRange(newSubjects);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError("", "Failed to save changes.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            return RedirectToAction(nameof(Details));
        }

        // GET: Tutor/DeleteAccount
        [HttpGet]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Details));

            return View(tutor);
        }

        // POST: Tutor/DeleteAccount
        [HttpPost, ActionName("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor != null)
            {
                _context.TutorSubjects.RemoveRange(tutor.TutorSubjects);
                _context.Tutors.Remove(tutor);
                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);
    

            return RedirectToAction("Index", "Home");
        }

        // POST: Tutor/JoinChannel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinChannel(int subjectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Details));

            var isApprovedForSubject = tutor.TutorSubjects.Any(ts => ts.SubjectId == subjectId && ts.Approved);

            if (!isApprovedForSubject)
            {
                TempData["ErrorMessage"] = "You are not approved to tutor this subject.";
                return RedirectToAction(nameof(Details));
            }

            TempData["SuccessMessage"] = "Successfully joined the subject channel!";
            return RedirectToAction(nameof(Details));
        }

        // GET: Tutor/UploadMaterial
        [HttpGet]
        public async Task<IActionResult> UploadMaterial(int subjectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors
                .Include(t => t.TutorSubjects)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Details));

            var isApprovedForSubject = tutor.TutorSubjects.Any(ts => ts.SubjectId == subjectId && ts.Approved);

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

        // POST: Tutor/UploadMaterial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMaterial(TutorMaterial model )
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            if (tutor == null) return RedirectToAction(nameof(Details));

            var isApprovedForSubject = await _context.TutorSubjects
                .AnyAsync(ts => ts.TutorId == tutor.Id && ts.SubjectId == model.SubjectId && ts.Approved);

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

            // TODO: Add file upload handling if needed

            model.UploadDate = DateTime.UtcNow;
            model.TutorId = tutor.Id;

            _context.TutorMaterials.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Material uploaded successfully!";
            return RedirectToAction(nameof(Details));
        }
    }
}
