using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaWeb.Services.Notifications;

namespace CinemaWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DbContexts _context;
        private readonly INotificationSubject _notificationSubject;

        public OrdersController(DbContexts context, INotificationSubject notificationSubject)
        {
            _context = context;
            _notificationSubject = notificationSubject;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderTime)
                .ThenByDescending(o => o.IdOrder);

            return View(await orders.ToListAsync());
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Tickets)
                    .ThenInclude(o => o.Seat)
                .Include(o => o.OrderCombos)
                    .ThenInclude(oc => oc.Combo)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.IdOrder == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            ViewData["IdUser"] = _context.Users.ToList();
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderTime = DateTime.Now;
                _context.Add(order);
                await _context.SaveChangesAsync();
                _notificationSubject.Publish("Tạo đơn hàng thành công!", "success");
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdUser"] = _context.Users.ToList();
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            ViewData["IdUser"] = _context.Users.ToList();
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.IdOrder) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
                _notificationSubject.Publish("Cập nhật đơn hàng thành công!", "info");
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdUser"] = _context.Users.ToList();
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.IdOrder == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            _notificationSubject.Publish("Xóa đơn hàng thành công!", "warning");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult History()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var orders = _context.Orders
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
                .Include(o => o.Tickets)
                .Include(o => o.Payments)
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
            return RedirectToAction("HistoryDetails", "Orders", new { id = id });
        }
    }
}
