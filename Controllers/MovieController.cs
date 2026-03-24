using CinemaWeb.Models;
using CinemaWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly DbContexts _context;

        public MovieController(IMovieRepository movieRepository, DbContexts context)
        {
            _movieRepository = movieRepository;
            _context = context;
        }

        public IActionResult Detail(int id)
        {
            // 1. Phải nạp đầy đủ các bảng liên quan để View có dữ liệu rạp/phòng
            var movie = _context.Movies
                .Include(m => m.Showtimes)
                    .ThenInclude(s => s.ScreeningRoom)
                        .ThenInclude(r => r.Cinema)
                .FirstOrDefault(m => m.IdMovie == id);

            if (movie == null) return NotFound();

            // 2. Lấy danh sách các khung giờ chiếu duy nhất để khách chọn (không phân biệt ngày)
            var uniqueTimeSlots = movie.Showtimes
                .GroupBy(s => new { s.StartTime, s.ScreeningRoom.Cinema.CinemaName })
                .Select(g => g.First())
                .OrderBy(s => s.StartTime)
                .ToList();

            ViewBag.UniqueTimeSlots = uniqueTimeSlots;

            return View(movie);
        }

        public IActionResult Search(string keyword)
        {
            var movies = _context.Movies
                .Where(x => x.MovieName.Contains(keyword))
                .Select(x => new {
                    x.IdMovie,
                    x.MovieName,
                    x.Poster
                })
                .Take(5)
                .ToList();

            return Json(movies);
        }

        public IActionResult GetHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) return Json(new List<object>());

            var history = _context.SearchHistories
                .Include(x => x.Movie)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SearchTime)
                .AsEnumerable() // 🔥 giữ lại
                .GroupBy(x => x.IdMovie) // 🔥 gom theo phim
                .Select(g => g.First())  // 🔥 lấy cái mới nhất
                .Select(x => new {
                    idMovie = x.IdMovie,
                    movieName = x.Movie.MovieName,
                    poster = x.Movie.Poster // 🔥 THÊM
                })
                .Take(5)
                .ToList();

            return Json(history);
        }

        public IActionResult SaveHistory(int movieId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) return Ok();

            var history = new SearchHistory
            {
                UserId = userId.Value,
                IdMovie = movieId,
                SearchTime = DateTime.Now
            };

            _context.SearchHistories.Add(history);
            _context.SaveChanges();

            return Ok();
        }
        
        public IActionResult DeleteHistory(int movieId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) return Ok();

            var items = _context.SearchHistories
                .Where(x => x.UserId == userId && x.IdMovie == movieId)
                .ToList();

            _context.SearchHistories.RemoveRange(items);
            _context.SaveChanges();

            return Ok();
        }
    }
}
