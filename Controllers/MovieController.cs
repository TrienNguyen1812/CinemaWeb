using CinemaWeb.Models;
using CinemaWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;

        public MovieController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public IActionResult Detail(int id)
        {
            var movie = _movieRepository.GetMovieDetail(id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }
    }
}
