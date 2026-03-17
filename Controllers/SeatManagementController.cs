using CinemaWeb.Models;
using CinemaWeb.Services.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Controllers
{
    public class SeatManagementController : Controller
    {
        private readonly DbContexts _context;

        public IActionResult GenerateSeats()
        {
            ViewBag.Rooms = _context.ScreeningRooms.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult GenerateSeats(int roomId)
        {
            if (roomId == 0)
            {
                TempData["Error"] = "Vui lòng chọn phòng";
                return RedirectToAction("Index");
            }

            var builder = new SeatLayoutBuilder();
            var director = new SeatLayoutDirector();

            director.BuildStandardRoom(builder, roomId);

            var seats = builder.Build();

            _context.Seats.AddRange(seats);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public SeatManagementController(DbContexts context)
        {
            _context = context;
        }

        // GET: Seat
        public async Task<IActionResult> Index()
        {
            var seats = _context.Seats
                .Include(s => s.ScreeningRoom)
                .ThenInclude(r => r.Cinema);
            return View(await seats.ToListAsync());
        }

        // GET: Seat/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var seat = await _context.Seats
                       .Include(s => s.ScreeningRoom)
                       .ThenInclude(r => r.Cinema)
                       .FirstOrDefaultAsync(m => m.IdSeat == id);

            if (seat == null) return NotFound();

            return View(seat);
        }

        // GET: Seat/Create
        public IActionResult Create()
        {
            ViewBag.Rooms = _context.ScreeningRooms.ToList();
            return View();
        }

        // POST: Seat/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seat seat)
        {
            // ❗ Check thiếu thuộc tính
            if (string.IsNullOrEmpty(seat.SeatRow) ||
                string.IsNullOrEmpty(seat.SeatNumber) ||
                string.IsNullOrEmpty(seat.TypeSeat))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin ghế");
            }

            // ❗ Check trùng ghế trong cùng rạp
            bool isExist = await _context.Seats.AnyAsync(s =>
                s.IdRoom == seat.IdRoom &&
                s.SeatRow == seat.SeatRow &&
                s.SeatNumber == seat.SeatNumber);

            if (isExist)
            {
                ModelState.AddModelError("", "Ghế này đã tồn tại trong rạp");
            }

            if (ModelState.IsValid)
            {
                _context.Add(seat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Cinemas = _context.Cinemas.ToList();
            return View(seat);
        }

        // GET: Seat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var seat = await _context.Seats.FindAsync(id);
            if (seat == null) return NotFound();

            ViewBag.Cinemas = _context.Cinemas.ToList();
            return View(seat);
        }

        // POST: Seat/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seat seat)
        {
            if (id != seat.IdSeat) return NotFound();

            // Check trùng ghế (trừ chính nó)
            bool isExist = await _context.Seats.AnyAsync(s =>
                s.IdSeat != seat.IdSeat &&
                s.IdRoom == seat.IdRoom &&
                s.SeatRow == seat.SeatRow &&
                s.SeatNumber == seat.SeatNumber);

            if (isExist)
            {
                ModelState.AddModelError("", "Ghế này đã tồn tại trong rạp");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Seats.Any(e => e.IdSeat == seat.IdSeat))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Cinemas = _context.Cinemas.ToList();
            return View(seat);
        }

        // GET: Seat/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var seat = await _context.Seats
                       .Include(s => s.ScreeningRoom)
                       .ThenInclude(r => r.Cinema)
                       .FirstOrDefaultAsync(m => m.IdSeat == id);

            if (seat == null) return NotFound();

            return View(seat);
        }

        // POST: Seat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seat = await _context.Seats.FindAsync(id);

            // ❗ Không cho xóa nếu ghế đã có vé
            bool hasTicket = await _context.Tickets.AnyAsync(t => t.IdSeat == id);
            if (hasTicket)
            {
                TempData["Error"] = "Không thể xóa ghế đã có vé";
                return RedirectToAction(nameof(Index));
            }

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
