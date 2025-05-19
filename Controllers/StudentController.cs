using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPortal.Data;
using StudentPortal.Models;


namespace StudentPortal.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentPortalDbContext _context;
        public StudentsController(StudentPortalDbContext context) => _context = context;

        public async Task<IActionResult> Index() => View(await _context.Students.Include(s => s.UserProfile).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.UserProfile)
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        public IActionResult Create()
        {
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,UserProfileId")] Student student)
        {
            if (!ModelState.IsValid)
            {
                ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Username", student.UserProfileId);
                return View(student);
            }
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Username", student.UserProfileId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,UserProfileId")] Student student)
        {
            if (id != student.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Username", student.UserProfileId);
                return View(student);
            }

            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
