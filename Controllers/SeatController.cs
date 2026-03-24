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

        public IActionResult SelectSeat(int id, DateTime? date)
        {
            // CẬP NHẬT: Thêm các lệnh .Include để nạp dữ liệu liên quan
            var showtime = _context.Showtimes
                .Include(s => s.Movie)                          // Để hiển thị tên phim
                .Include(s => s.ScreeningRoom)                  // Nạp phòng chiếu (Tránh lỗi Null ở Model.ScreeningRoom)
                    .ThenInclude(r => r.Seats)                  // Nạp danh sách ghế (Tránh lỗi Null ở .Seats)
                        .ThenInclude(seat => seat.Tickets)      // Nạp vé để check ghế đã đặt hay chưa
                            .ThenInclude(t => t.Order)                 // Nạp đơn hàng để check trạng thái vé
                .FirstOrDefault(s => s.IdShowtime == id);
                
            ViewBag.IsNightShow = showtime.StartTime.Hours >= 22 || showtime.StartTime.Hours < 5;

            if (showtime == null) 
            {
                return NotFound();
            }

            // Logic xử lý ngày tháng (như đã hướng dẫn ở bước trước)
            var selectedDate = date ?? DateTime.Today;
            var dates = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(i)).ToList();

            ViewBag.AvailableDates = dates;
            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");

            return View(showtime);
        }
    }
}
