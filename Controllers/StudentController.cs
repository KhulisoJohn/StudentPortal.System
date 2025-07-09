using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalSystem.Data;
using StudentPortalSystem.Enums;
using StudentPortalSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var existingStudent = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (existingStudent != null)
                return RedirectToAction(nameof(Details));

            ViewBag.AllSubjects = await _context.Subjects.ToListAsync();

            var model = new Student
            {
                EnrollmentDate = DateTime.UtcNow,
                Status = UserStatus.Active
            };

            return View(model);
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model, int[] selectedSubjects)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var existingStudent = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (existingStudent != null)
                return RedirectToAction(nameof(Details));

            if (!ModelState.IsValid)
            {
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            if (model.Grade >= GradeRange.Grade10 && model.Grade <= GradeRange.Grade12)
            {
                if (selectedSubjects == null || selectedSubjects.Length != 4)
                {
                    ModelState.AddModelError("", "Students in grades 10 to 12 must select exactly 4 subjects.");
                    ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                    return View(model);
                }
            }
            else if (model.Grade >= GradeRange.Grade4 && model.Grade <= GradeRange.Grade9)
            {
                selectedSubjects = await _context.Subjects.Select(s => s.Id).ToArrayAsync();
            }
            else
            {
                ModelState.AddModelError("Grade", "Only grades 4 to 12 are supported.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            var age = DateTime.Today.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            model.ApplicationUserId = user.Id;
            model.EnrollmentDate = DateTime.UtcNow;
            model.Status = UserStatus.Active;
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error: " + ex.Message);
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Details));
        }

        // GET: Student/Details
        [HttpGet]
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

        // GET: Student/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
                return RedirectToAction(nameof(Create));

            ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
            ViewBag.SelectedSubjects = student.StudentSubjects.Select(ss => ss.SubjectId).ToArray();

            return View(student);
        }

        // POST: Student/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student model, int[] selectedSubjects)
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.Id == model.Id && s.ApplicationUserId == user.Id);

            if (student == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            if (model.Grade >= GradeRange.Grade10 && model.Grade <= GradeRange.Grade12)
            {
                if (selectedSubjects.Length != 4)
                {
                    ModelState.AddModelError("", "You must select exactly 4 subjects.");
                    ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                    ViewBag.SelectedSubjects = selectedSubjects;
                    return View(model);
                }
            }
            else if (model.Grade >= GradeRange.Grade4 && model.Grade <= GradeRange.Grade9)
            {
                selectedSubjects = await _context.Subjects.Select(s => s.Id).ToArrayAsync();
            }

            var age = DateTime.Today.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            student.DateOfBirth = model.DateOfBirth;
            student.Grade = model.Grade;
            student.CanJoinSubjectChannels = age >= 12;

            _context.StudentSubjects.RemoveRange(student.StudentSubjects);

            var newSubjects = selectedSubjects.Select(subjectId => new StudentSubject
            {
                StudentId = student.Id,
                SubjectId = subjectId
            });

            _context.StudentSubjects.AddRange(newSubjects);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError("", "Failed to save. Try again.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            return RedirectToAction(nameof(Details));
        }

        // GET: Student/DeleteAccount
        [HttpGet]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
                return RedirectToAction(nameof(Create));

            return View(student);
        }

        // POST: Student/DeleteAccount
        [HttpPost, ActionName("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student != null)
            {
                _context.StudentSubjects.RemoveRange(student.StudentSubjects);
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Admin-only list of students
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.ApplicationUser)
                .Include(s => s.StudentSubjects)
                .ThenInclude(ss => ss.Subject)
                .ToListAsync();

            return View(students);
        }
    }
}
