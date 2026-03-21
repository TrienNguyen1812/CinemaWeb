using CinemaWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWeb.Controllers
{
    public class CustomerMovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private const int PageSize = 10;
        private const int MaxPreviewPages = 3;

        public CustomerMovieController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public IActionResult Index()
        {
            return View(_movieRepository.GetNowShowingMovies());
        }

        public IActionResult Detail(int id)
        {
            var movie = _movieRepository.GetById(id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }
        // 👉 DÙNG CHO HOME (preview)
        public IActionResult NowShowingPreview(int page = 1)
        {
            // Giới hạn trang preview nếu cần
            if (page > MaxPreviewPages) page = MaxPreviewPages;

            // Lấy danh sách phim đã lọc "Đang chiếu" từ Repository
            var movies = _movieRepository.GetNowShowingMoviesPaged(page, PageSize);

            // Tính toán tổng số trang dựa trên số phim thực tế có Status "Đang chiếu"
            int totalShowingMovies = _movieRepository.CountNowShowingMovies();
            int calculatedPages = (int)Math.Ceiling(totalShowingMovies / (double)PageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Min(MaxPreviewPages, calculatedPages);

            return View(movies);
        }

        public IActionResult ComingSoonPreview(int page = 1)
        {
            if (page > MaxPreviewPages) page = MaxPreviewPages;

            var movies = _movieRepository.GetComingSoonMoviesPaged(page, PageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Min(MaxPreviewPages, 
                (int)Math.Ceiling(_movieRepository.CountComingSoonMovies() / (double)PageSize));

            return View(movies);
        }

        // 👉 DÙNG CHO "XEM THÊM"
        public IActionResult NowShowing()
        {
            var movies = _movieRepository.GetAllMovies();
            return View(movies);
        }
    }
}
