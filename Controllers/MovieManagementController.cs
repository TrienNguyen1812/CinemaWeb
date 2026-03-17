using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CinemaWeb.Controllers
{
    public class MovieManagementController : Controller
    {
        private readonly DbContexts _context;
        private readonly ILogger<MovieManagementController> _logger;

        public MovieManagementController(DbContexts context, ILogger<MovieManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // READ - danh sách phim
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        // CREATE - GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - POST
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile posterFile)
        {
            if (posterFile == null || posterFile.Length == 0)
            {
                ModelState.AddModelError("Poster", "Poster là bắt buộc");
                return View(movie);
            }

            // 👉 LẤY TÊN FILE GỐC
            var fileName = Path.GetFileName(posterFile.FileName); // marvel.jpg

            // 👉 ĐƯỜNG DẪN LƯU
            var path = Path.Combine(Directory.GetCurrentDirectory(),
                                    "wwwroot/images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await posterFile.CopyToAsync(stream);
            }

            // 👉 GÁN CHO DB
            movie.Poster = fileName;

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // UPDATE - GET
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        // UPDATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.IdMovie) return NotFound();

            if (!ModelState.IsValid)
                return View(movie);

            _context.Update(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // DELETE - GET
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
