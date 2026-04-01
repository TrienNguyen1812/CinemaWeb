using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaWeb.Services.Notifications;

namespace CinemaWeb.Controllers
{
    public class HistoryController : Controller
    {
        private readonly DbContexts _context;
        private readonly INotificationSubject _notificationSubject;

        public HistoryController(DbContexts context, INotificationSubject notificationSubject)
        {
            _context = context;
            _notificationSubject = notificationSubject;
        }

        public IActionResult History()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var orders = _context.Orders
                .Include(o => o.Tickets)
                .Include(o => o.OrderCombos)
                    .ThenInclude(oc => oc.Combo)
                .Where(o => o.IdUser == userId)
                .OrderByDescending(o => o.OrderTime)
                .ToList();

            return View(orders);
        }

        public async Task<IActionResult> HistoryDetails(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Payments)
                .Include(o => o.OrderCombos).ThenInclude(oc => oc.Combo)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Seat)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.ScreeningRoom)
                .FirstOrDefaultAsync(o => o.IdOrder == id);

            if (order == null) return NotFound();

            return View(order);
        }

        public IActionResult RedirectOrder(int id)
        {
            var order = _context.Orders.Find(id);

            if (order == null) return NotFound();

            // 👉 nếu chưa thanh toán → quay lại checkout
            if (order.Status == PaymentConstants.OrderDelay)
            {
                return RedirectToAction("Checkout", "Payment", new { orderId = id });
            }

            // 👉 nếu đã thanh toán hoặc huỷ → xem chi tiết
            return RedirectToAction("HistoryDetails", "History", new { id = id });
        }
    }
}
