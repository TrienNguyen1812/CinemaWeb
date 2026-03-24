using CinemaWeb.Models;
using CinemaWeb.Services.Builders;
using CinemaWeb.Services.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Controllers
{
    public class SeatManagementController : Controller
    {
        private readonly DbContexts _context;
        private readonly INotificationSubject _notificationSubject;

        public SeatManagementController(DbContexts context, INotificationSubject notificationSubject)
        {
            _context = context;
            _notificationSubject = notificationSubject;
        }

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
            // Logic kiểm tra dữ liệu
            if (string.IsNullOrEmpty(seat.SeatRow) || 
                string.IsNullOrEmpty(seat.SeatNumber) || 
                string.IsNullOrEmpty(seat.TypeSeat))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin ghế.");
            }

             if (seat.IdRoom == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn phòng.");
            }

            bool isExist = await _context.Seats.AnyAsync(s =>
            s.IdRoom == seat.IdRoom && // Phải khớp mã phòng
            s.SeatRow == seat.SeatRow &&
            s.SeatNumber == seat.SeatNumber);

            if (isExist)
            {
                ModelState.AddModelError("", $"Ghế {seat.SeatRow}{seat.SeatNumber} đã tồn tại trong phòng này.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(seat);
                await _context.SaveChangesAsync();
                _notificationSubject.Publish("Thêm ghế thành công!", "success");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Rooms = _context.ScreeningRooms.ToList(); 
            return View(seat);
        }

        // GET: Seat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var seat = await _context.Seats.FindAsync(id);
            if (seat == null) return NotFound();

            // ❗ SỬA TẠI ĐÂY: Dùng ViewBag.Rooms cho đồng bộ với View
            ViewBag.Rooms = _context.ScreeningRooms.ToList(); 
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
                ModelState.AddModelError("", $"Ghế {seat.SeatRow}{seat.SeatNumber} đã tồn tại trong phòng này.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seat);
                    await _context.SaveChangesAsync();
                    _notificationSubject.Publish("Cập nhật ghế thành công!", "info");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Seats.Any(e => e.IdSeat == seat.IdSeat)) 
                        _notificationSubject.Publish("Ghế không tồn tại!", "error");
                }
            }

            ViewBag.Rooms = _context.ScreeningRooms.ToList();
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
            _notificationSubject.Publish("Xóa ghế thành công!", "warning");
            return RedirectToAction(nameof(Index));
        }
    }
}
