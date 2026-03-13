using CinemaWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWeb.Controllers
{
    public class CustomerMovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private const int PageSize = 4;
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
            if (page > MaxPreviewPages)
                page = MaxPreviewPages;

            var movies = _movieRepository.GetNowShowingMoviesPaged(page, PageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Min(
                MaxPreviewPages,
                (int)Math.Ceiling(
                    _movieRepository.CountNowShowingMovies() / (double)PageSize
                )
            );

            return View(movies);
        }

        // 👉 DÙNG CHO "XEM THÊM"
        public IActionResult NowShowing()
        {
            var movies = _movieRepository.GetNowShowingMovies();
            return View(movies);
        }
    }
}
