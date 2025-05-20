using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Controllers;
using StudentPortal.Data;
using StudentPortal.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortal.Controllers
{
    public class StudentCoursesController : Controller
    {
        private readonly StudentPortalDbContext _context;

        public StudentCoursesController(StudentPortalDbContext context)
        {
            _context = context;
        }

        // GET: StudentCourses
        public async Task<IActionResult> Index()
        {
            var studentCourses = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .ToListAsync();

            return View(studentCourses);
        }

        // GET: StudentCourses/Create
        public IActionResult Create()
        {
            // Populate dropdown for Students and Courses
            ViewBag.Students = new SelectList(_context.Students.OrderBy(s => s.FullName), "Id", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses.OrderBy(c => c.Title), "Id", "Title");

            return View();
        }

        // POST: StudentCourses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,CourseId")] StudentCourse studentCourse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If model state invalid, reload dropdowns to not break the form
            ViewBag.Students = new SelectList(_context.Students.OrderBy(s => s.FullName), "Id", "FullName", studentCourse.StudentId);
            ViewBag.Courses = new SelectList(_context.Courses.OrderBy(c => c.Title), "Id", "Title", studentCourse.CourseId);

            return View(studentCourse);
        }

        // GET: StudentCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var studentCourse = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.Id == id);

            if (studentCourse == null) return NotFound();

            // Defensive check: if student or course is null, handle gracefully
            if (studentCourse.Student == null)
                studentCourse.Student = new Student { FullName = "[No Student Assigned]" };

            if (studentCourse.Course == null)
                studentCourse.Course = new Course { Title = "[No Course Assigned]" };

            return View(studentCourse);
        }

        // POST: StudentCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentCourse = await _context.StudentCourses.FindAsync(id);
            if (studentCourse != null)
            {
                _context.StudentCourses.Remove(studentCourse);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
