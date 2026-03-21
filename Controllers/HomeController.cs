using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CinemaWeb.Models;
using CinemaWeb.Services;

namespace CinemaWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieRepository _movieRepository;

    public HomeController(ILogger<HomeController> logger, IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Lấy toàn bộ phim đang chiếu và sắp chiếu từ Repository
        var nowShowing = _movieRepository.GetNowShowingMovies(); // Hàm này trong Repo của bạn đã lấy hết rồi
        var comingSoon = _movieRepository.GetComingSoonMovies();

        // Gán vào ViewBag ĐẦY ĐỦ, không dùng .Take(4) ở đây nữa
        ViewBag.NowShowing = nowShowing; 
        ViewBag.ComingSoon = comingSoon; 

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
