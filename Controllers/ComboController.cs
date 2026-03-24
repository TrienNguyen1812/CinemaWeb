using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWeb.Controllers
{
    public class ComboController : Controller
    {
        private readonly DbContexts _context;

        public ComboController(DbContexts context)
        {
            _context = context;
        }

        public IActionResult SelectCombo(int showtimeId, List<int> seatIds, string watchDate)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var combos = _context.Combos.ToList();

            ViewBag.SeatIds = seatIds;
            ViewBag.ShowtimeId = showtimeId;
            ViewBag.WatchDate = watchDate;

            HttpContext.Session.SetString("SelectedWatchDate", watchDate);

            return View(combos);
        }
    }
}
