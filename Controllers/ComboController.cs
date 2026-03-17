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

        public IActionResult SelectCombo(int showtimeId, List<int> seatIds)
        {
            var combos = _context.Combos.ToList();

            ViewBag.SeatIds = seatIds;
            ViewBag.ShowtimeId = showtimeId;

            return View(combos);
        }
    }
}
