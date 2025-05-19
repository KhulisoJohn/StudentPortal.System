using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly StudentPortalDbContext _context;
        public UserProfilesController(StudentPortalDbContext context) => _context = context;

        public async Task<IActionResult> Index() => View(await _context.UserProfiles.ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == id);
            if (userProfile == null) return NotFound();

            return View(userProfile);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Email")] UserProfile userProfile)
        {
            if (!ModelState.IsValid) return View(userProfile);
            _context.Add(userProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userProfile = await _context.UserProfiles.FindAsync(id);
            if (userProfile == null) return NotFound();

            return View(userProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email")] UserProfile userProfile)
        {
            if (id != userProfile.Id) return NotFound();

            if (!ModelState.IsValid) return View(userProfile);

            try
            {
                _context.Update(userProfile);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserProfiles.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == id);
            if (userProfile == null) return NotFound();

            return View(userProfile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);
            if (userProfile != null)
            {
                _context.UserProfiles.Remove(userProfile);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
