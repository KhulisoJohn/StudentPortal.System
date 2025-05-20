using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace StudentPortal.Controllers
{
    public class BooksController : Controller
    {
        private readonly StudentPortalDbContext _context;
        public BooksController(StudentPortalDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var books = _context.Books.Include(b => b.Author).Include(b => b.Course);
            return View(await books.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName");
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,AuthorId,CourseId")] Book book)
        {
            if (!ModelState.IsValid)
            {
                ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
                ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", book.CourseId);
                return View(book);
            }
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", book.CourseId);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,AuthorId,CourseId")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
                ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", book.CourseId);
                return View(book);
            }

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
