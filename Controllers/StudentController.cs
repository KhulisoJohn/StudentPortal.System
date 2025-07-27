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

        // GET: Student/Details (read & create if missing)
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);

            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .ThenInclude(ss => ss.Subject)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
            {
                // Auto-create new Student entity with minimal data
                student = new Student
                {
                    ApplicationUserId = user.Id,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = UserStatus.Active,
                    DateOfBirth = DateTime.Today.AddYears(-15), // default DOB, adjust as needed
                    Grade = GradeRange.Grade10, // default grade, adjust as needed
                    CanJoinSubjectChannels = true // default true, adjust based on age if needed
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                // Optionally assign default subjects here if needed
            }

            return View(student);
        }

        // GET: Student/Edit (show form with subjects)
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
                return RedirectToAction(nameof(Details)); // will create student

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

            // Validate subjects count based on grade
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
            else
            {
                ModelState.AddModelError("Grade", "Only grades 4 to 12 are supported.");
                ViewBag.AllSubjects = await _context.Subjects.ToListAsync();
                ViewBag.SelectedSubjects = selectedSubjects;
                return View(model);
            }

            var age = DateTime.Today.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            // Update student properties
            student.DateOfBirth = model.DateOfBirth;
            student.Grade = model.Grade;
            student.CanJoinSubjectChannels = age >= 12;

            // Update subjects
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
                return RedirectToAction(nameof(Details));

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
