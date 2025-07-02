using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalSystem.Data;
using StudentPortalSystem.Models;

namespace StudentPortalSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly StudentPortalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(StudentPortalDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Student/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var existingStudent = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (existingStudent != null)
                return RedirectToAction(nameof(Details));

            ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model, int[] selectedSubjects)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            var existingStudent = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (existingStudent != null)
                return RedirectToAction(nameof(Details));

            if (model.Grade >= 10 && model.Grade <= 11)
            {
                if (selectedSubjects == null || selectedSubjects.Length != 4)
                {
                    ModelState.AddModelError("", "Students in grades 10 to 11 must select exactly 4 subjects.");
                    ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                    return View(model);
                }
            }
            else if (model.Grade >= 4 && model.Grade <= 9)
            {
                selectedSubjects = await _context.Subjects.Select(s => s.Id).ToArrayAsync();
            }
            else
            {
                ModelState.AddModelError("Grade", "Invalid grade. Only grades 4 to 11 are supported.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            var age = DateTime.Today.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            model.ApplicationUserId = user.Id;
            model.EnrollmentDate = DateTime.UtcNow;
            model.CanJoinSubjectChannels = age >= 12;

            try
            {
                _context.Students.Add(model);
                await _context.SaveChangesAsync();

                var studentSubjects = selectedSubjects.Select(subjectId => new StudentSubject
                {
                    StudentId = model.Id,
                    SubjectId = subjectId
                });

                _context.StudentSubjects.AddRange(studentSubjects);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Details));
        }

        // GET: Student/Details
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .ThenInclude(ss => ss.Subject)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
                return RedirectToAction(nameof(Create));

            return View(student);
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(static s => s.ApplicationUser)
                .Include(static s => s.StudentSubjects)
                .ThenInclude(static ss => ss.Subject)
                .ToListAsync();

            return View(students);
        }
    }
}
