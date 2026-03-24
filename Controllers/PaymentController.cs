using CinemaWeb.Models;
using CinemaWeb.Services.Commands;
using CinemaWeb.Services.Factory;
using CinemaWeb.Services.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using CinemaWeb.Services.Notifications;

namespace CinemaWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DbContexts _context;
        private readonly CreateOrderCommandHandler _handler;
        private readonly INotificationSubject _notificationSubject;
        private readonly PaymentService _paymentService;

        public PaymentController(DbContexts context, CreateOrderCommandHandler handler,
            INotificationSubject notificationSubject,
            IEnumerable<INotificationObserver> observers,
            PaymentService paymentService)
        {
            _context = context;
            _handler = handler;
            _notificationSubject = notificationSubject;
            _paymentService = paymentService;

            foreach (var observer in observers)
            {
                _notificationSubject.Attach(observer);
            }
        }

        [HttpPost]
        public IActionResult Checkout(IFormCollection form)
        {
            var expiredOrders = _context.Orders
                .Where(o => o.Status == PaymentConstants.OrderDelay && o.ExpiredAt < DateTime.Now)
                .ToList();

            foreach (var o in expiredOrders)
            {
                o.Status = PaymentConstants.OrderCancelled;
            }

            _context.SaveChanges();
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            if (!int.TryParse(form["showtimeId"], out var showtimeId))
                return BadRequest("showtimeId không hợp lệ");

            DateTime watchDate = DateTime.Now;
            if (DateTime.TryParse(form["watchDate"], out var parsedDate))
            {
                watchDate = parsedDate;
            }

            var seatIds = form["seatIds"].Select(x => int.TryParse(x, out var s) ? s : 0).Where(x => x > 0).ToList();
            var bookedSeats = _context.Tickets
                .Include(t => t.Order)
                .Where(t => t.IdShowtime == showtimeId && seatIds.Contains(t.IdSeat) && t.WatchDate.Date == watchDate.Date)
                .Where(t => t.Order.Status != PaymentConstants.OrderCancelled) // Không tính vé của đơn đã hủy
                .Select(t => t.Seat.SeatRow + t.Seat.SeatNumber)
                .ToList();

            if (bookedSeats.Any())
            {
                string seatNames = string.Join(", ", bookedSeats);
                _notificationSubject.Publish($"Rất tiếc, ghế ({seatNames}) vừa có người khác đặt. Vui lòng chọn ghế khác!", "danger");
                
                // Quay lại trang chọn ghế của suất chiếu đó
                return RedirectToAction("SelectSeat", "Seat", new { id = showtimeId });
            }
            var combos = new Dictionary<int, int>();

            foreach (var key in form.Keys)
            {
                if (!key.StartsWith("combos["))
                    continue;

                var start = key.IndexOf("[") + 1;
                var end = key.IndexOf("]");
                if (start <= 0 || end <= start)
                    continue;

                if (int.TryParse(key.Substring(start, end - start), out var comboId)
                    && int.TryParse(form[key], out var qty)
                    && qty > 0)
                {
                    combos[comboId] = qty;
                }
            }

            ViewBag.Debug = new {
                showtimeId,
                seats = seatIds,
                combos
            };

            var command = new CreateOrderCommand
            {
                IdUser = userId.Value,
                IdShowtime = showtimeId,
                SeatIds = seatIds,
                Combos = combos,
                WatchDate = watchDate
            };

            var orderId = _handler.Handle(command);

            _notificationSubject.Publish($"Tạo vé thành công.", "success");

            var savedCombos = _context.OrderCombos.Where(oc => oc.IdOrder == orderId).Select(oc => new { oc.IdCombo, oc.Quantity }).ToList();
            ViewBag.SavedOrderCombos = savedCombos;

            var vm = _paymentService.GetPaymentDetails(orderId);
    
            if (vm == null) return NotFound();

            // Nếu bạn vẫn cần dùng ViewBag cho View hiện tại:
            ViewBag.WatchDate = vm.ShowtimeDate.ToString("yyyy-MM-dd");
            ViewBag.OrderComboCount = vm.Combos.Count;

            return View(vm);
        }

        [HttpGet]
        public IActionResult Checkout(int orderId)
        {
            var vm = _paymentService.GetPaymentDetails(orderId);
    
            if (vm == null) return NotFound();

            // Nếu bạn vẫn cần dùng ViewBag cho View hiện tại:
            ViewBag.WatchDate = vm.ShowtimeDate.ToString("yyyy-MM-dd");
            ViewBag.OrderComboCount = vm.Combos.Count;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, string paymentType, decimal total)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null) return NotFound();

            // Thêm lại đoạn này để tránh khách thanh toán cho đơn đã hết hạn
            if (order.ExpiredAt < DateTime.Now)
            {
                order.Status = PaymentConstants.OrderCancelled;
                _context.SaveChanges();
                _notificationSubject.Publish("Đơn hàng đã hết hạn thanh toán.", "danger");
                return RedirectToAction("Index", "Home");
            }
            try 
            {
                // Chỉ cần gọi 1 dòng duy nhất để xử lý thanh toán và lưu DB
                var paymentProcessor = PaymentFactory.CreatePayment(paymentType);
                _paymentService.ExecutePayment(orderId, paymentType);

                string msg = paymentProcessor.GetSuccessMessage();
                _notificationSubject.Publish(msg, "success");
                return RedirectToAction("Success", new { id = orderId });
            }
            catch (Exception ex)
            {
                _notificationSubject.Publish(ex.Message, "danger");
                return RedirectToAction("Checkout", new { orderId = orderId });
            }
        }

        [HttpGet]
        public IActionResult Success(int id)
        {
            var vm = _paymentService.GetPaymentDetails(id);
    
            if (vm == null) return NotFound();

            // Nếu bạn vẫn cần dùng ViewBag cho View hiện tại:
            ViewBag.WatchDate = vm.ShowtimeDate.ToString("yyyy-MM-dd");
            ViewBag.OrderComboCount = vm.Combos.Count;

            return View(vm);
        }
        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
                return NotFound();

            // chỉ huỷ khi chưa thanh toán
            if (order.Status == "Đã thanh toán")
                return BadRequest("Đơn đã thanh toán, không thể huỷ");

            order.Status = "Đã hủy";
            _context.SaveChanges();

            _notificationSubject.Publish($"Vé đã bị huỷ.", "warning");

            return RedirectToAction("Index", "Home");
        }
    }
}
