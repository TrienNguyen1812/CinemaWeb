using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly DbContexts _context;

        public UsersController(DbContexts context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.IdUser == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile posterFile)
        {
            // ===== MODELSTATE DEBUG =====
            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ MODELSTATE INVALID (MOVIE)");

                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"FIELD: {entry.Key} - ERROR: {error.ErrorMessage}");
                    }
                }

                return View(movie);
            }

            // ===== POSTER CHECK =====
            if (posterFile == null || posterFile.Length == 0)
            {
                Console.WriteLine("❌ POSTER FILE IS NULL OR EMPTY");
                ModelState.AddModelError("Poster", "Poster không được để trống");
                return View(movie);
            }

            try
            {
                // ===== UPLOAD POSTER =====
                string uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/movies"
                );

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                    Console.WriteLine("📁 CREATE FOLDER images/movies");
                }

                string fileName = Guid.NewGuid().ToString()
                                + Path.GetExtension(posterFile.FileName);

                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                movie.Poster = fileName;

                // ===== SAVE DB =====
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                Console.WriteLine("✅ CREATE MOVIE SUCCESS");
                Console.WriteLine($"NEW MOVIE ID = {movie.IdMovie}");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ EXCEPTION WHILE SAVING MOVIE");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);

                return View(movie);
            }
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.IdUser) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.IdUser == user.IdUser))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.IdUser == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
