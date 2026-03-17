using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Controllers
{
    public class SeatController : Controller
    {
        private readonly DbContexts _context;

        public SeatController(DbContexts context)
        {
            _context = context;
        }

        public IActionResult SelectSeat(int id) // id = IdShowtime
        {
            var showtime = _context.Showtimes
                .Include(s => s.ScreeningRoom)
                .ThenInclude(r => r.Seats)
                .FirstOrDefault(s => s.IdShowtime == id);

            if (showtime == null)
                return NotFound();

            return View(showtime);
        }
    }
}
