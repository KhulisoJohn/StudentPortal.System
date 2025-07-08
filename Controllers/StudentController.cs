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

        // GET: Student/Create or redirect if exists
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

            // Compare enum values directly
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
                ModelState.AddModelError("Grade", "Invalid grade. Only grades 4 to 12 are supported.");
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

        // GET: Student/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
                return RedirectToAction(nameof(Create));

            ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
            var selectedSubjectIds = student.StudentSubjects.Select(ss => ss.SubjectId).ToArray();

            var model = new Student
            {
                Id = student.Id,
                ApplicationUserId = student.ApplicationUserId,
                FullName = user.FullName,
                DateOfBirth = student.DateOfBirth,
                Grade = student.Grade,
                EnrollmentDate = student.EnrollmentDate,
                CanJoinSubjectChannels = student.CanJoinSubjectChannels
            };

            ViewBag.SelectedSubjects = selectedSubjectIds;
            return View(model);
        }

        // POST: Student/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student model, int[] selectedSubjects)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.Id == model.Id && s.ApplicationUserId == user.Id);

            if (student == null)
                return NotFound();

            if (model.Grade >= GradeRange.Grade10 && model.Grade <= GradeRange.Grade12)
            {
                if (selectedSubjects == null || selectedSubjects.Length != 4)
                {
                    ModelState.AddModelError("", "Students in grades 10 to 12 must select exactly 4 subjects.");
                    ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                    ViewBag.SelectedSubjects = selectedSubjects;
                    return View(model);
                }
            }
            else if (model.Grade >= GradeRange.Grade4 && model.Grade <= GradeRange.Grade9)
            {
                selectedSubjects = await _context.Subjects.Select(s => s.Id).ToArrayAsync();
            }
            else
            {
                ModelState.AddModelError("Grade", "Invalid grade. Only grades 4 to 12 are supported.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            var age = DateTime.Today.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            student.DateOfBirth = model.DateOfBirth;
            student.Grade = model.Grade;
            student.CanJoinSubjectChannels = age >= 12;
            student.EnrollmentDate = student.EnrollmentDate; // keep original

            // Update subjects
            _context.StudentSubjects.RemoveRange(student.StudentSubjects);

            var newStudentSubjects = selectedSubjects.Select(subjectId => new StudentSubject
            {
                StudentId = student.Id,
                SubjectId = subjectId
            });

            _context.StudentSubjects.AddRange(newStudentSubjects);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while updating. Please try again.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            return RedirectToAction(nameof(Details));
        }

        // GET: Student/Index (list all students)
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
