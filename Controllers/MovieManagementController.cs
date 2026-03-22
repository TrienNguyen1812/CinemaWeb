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
        public async Task<IActionResult> Create(Movie movie, IFormFile posterFile, IFormFile trailerFile)
        {
            if (posterFile == null || posterFile.Length == 0)
            {
                ModelState.AddModelError("Poster", "Poster là bắt buộc");
                return View(movie);
            }

            if (trailerFile == null || trailerFile.Length == 0)
            {
                ModelState.AddModelError("Trailer", "Trailer là bắt buộc");
                return View(movie);
            }

            // 👉 LẤY TÊN FILE GỐC
            var posterFileName = Path.GetFileName(posterFile.FileName); // marvel.jpg
            var trailerFileName = Path.GetFileName(trailerFile.FileName); // marvel_trailer.mp4

            // 👉 ĐƯỜNG DẪN LƯU
            var posterPath = Path.Combine(Directory.GetCurrentDirectory(),
                                    "wwwroot/images", posterFileName);
                var trailerPath = Path.Combine(Directory.GetCurrentDirectory(),
                                    "wwwroot/trailers", trailerFileName);

            using (var stream = new FileStream(posterPath, FileMode.Create))
            {
                await posterFile.CopyToAsync(stream);
            }

            using (var stream = new FileStream(trailerPath, FileMode.Create))
            {
                await trailerFile.CopyToAsync(stream);
            }

            // 👉 GÁN CHO DB
            movie.Poster = posterFileName;
            movie.Trailer = trailerFileName;

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
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? posterFile)
        {
            if (id != movie.IdMovie) return NotFound();

            // 1. Lấy dữ liệu phim cũ từ DB để giữ lại tên Poster cũ nếu không upload ảnh mới
            var existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.IdMovie == id);
            if (existingMovie == null) return NotFound();

            if (posterFile != null && posterFile.Length > 0)
            {
                // Xử lý upload ảnh mới giống như bên hàm Create
                var fileName = Path.GetFileName(posterFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }
                movie.Poster = fileName;
            }
            else
            {
                // Nếu không chọn ảnh mới, giữ lại tên ảnh cũ
                movie.Poster = existingMovie.Poster;
            }

            // 2. Kiểm tra ModelState (Xóa lỗi Poster nếu chúng ta đã có ảnh cũ)
            ModelState.Remove("posterFile"); 
            ModelState.Remove("Poster"); // Để tránh bị báo lỗi Required

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }
            return View(movie);
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
