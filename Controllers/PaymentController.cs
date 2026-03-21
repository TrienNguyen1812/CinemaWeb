using CinemaWeb.Models;
using CinemaWeb.Services.Commands;
using CinemaWeb.Services.Factory;
using CinemaWeb.Services.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace CinemaWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DbContexts _context;
        private readonly CreateOrderCommandHandler _handler;
        private readonly Services.Notifications.INotificationSubject _notificationSubject;

        public PaymentController(DbContexts context, CreateOrderCommandHandler handler,
            Services.Notifications.INotificationSubject notificationSubject,
            IEnumerable<Services.Notifications.INotificationObserver> observers)
        {
            _context = context;
            _handler = handler;
            _notificationSubject = notificationSubject;

            foreach (var observer in observers)
            {
                _notificationSubject.Attach(observer);
            }
        }

        [HttpPost]
        public IActionResult Checkout(IFormCollection form)
        {
            var expiredOrders = _context.Orders
                .Where(o => o.Status == "Chờ thanh toán" && o.ExpiredAt < DateTime.Now)
                .ToList();

            foreach (var o in expiredOrders)
            {
                o.Status = "Đã hủy";
            }

            _context.SaveChanges();
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            if (!int.TryParse(form["showtimeId"], out var showtimeId))
                return BadRequest("showtimeId không hợp lệ");

            var seatIds = form["seatIds"].Select(x => int.TryParse(x, out var s) ? s : 0).Where(x => x > 0).ToList();
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
                Combos = combos
            };

            var orderId = _handler.Handle(command);

            _notificationSubject.Publish($"Đặt vé thành công.", "success");

            var savedCombos = _context.OrderCombos.Where(oc => oc.IdOrder == orderId).Select(oc => new { oc.IdCombo, oc.Quantity }).ToList();
            ViewBag.SavedOrderCombos = savedCombos;

            var order = _context.Orders
                .Include(o => o.Tickets).ThenInclude(t => t.Seat)
                .Include(o => o.Tickets).ThenInclude(t => t.Showtime).ThenInclude(s => s.Movie)
                .Include(o => o.OrderCombos).ThenInclude(oc => oc.Combo)
                .FirstOrDefault(o => o.IdOrder == orderId);

            if (order == null)
                return NotFound();

            if (order.ExpiredAt < DateTime.Now)
            {
                order.Status = "Đã hủy";
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            var firstShowtime = order.Tickets.FirstOrDefault()?.Showtime;

            var orderCombos = order.OrderCombos?.ToList() ??
                _context.OrderCombos.Include(oc => oc.Combo).Where(oc => oc.IdOrder == order.IdOrder).ToList();

            var vm = new PaymentCheckoutViewModel
            {
                OrderId = order.IdOrder,
                MovieName = firstShowtime?.Movie?.MovieName ?? "",
                Poster = firstShowtime?.Movie?.Poster ?? "",
                ShowtimeDate = firstShowtime?.StartFilm ?? DateTime.Now,
                ShowtimeTime = firstShowtime != null ? firstShowtime.StartTime.ToString("HH:mm") : "",
                TicketCount = order.Tickets.Count,
                TicketTotal = order.Tickets.Sum(t => t.FinalPrice),
                ComboTotal = orderCombos.Sum(oc => (oc.Combo?.Price ?? _context.Combos.Find(oc.IdCombo)?.Price ?? 0m) * oc.Quantity),
                Total = order.TotalPrice,
                Status = order.Status
            };

            foreach (var ticket in order.Tickets)
            {
                if (ticket.Seat != null)
                    vm.SeatNames.Add($"{ticket.Seat.SeatRow}{ticket.Seat.SeatNumber}");
            }

            foreach (var orderCombo in orderCombos)
            {
                if (orderCombo.Quantity <= 0) continue;

                var comboInfo = orderCombo.Combo ?? _context.Combos.Find(orderCombo.IdCombo);

                vm.Combos.Add(new PaymentComboItem
                {
                    IdCombo = orderCombo.IdCombo,
                    ComboName = comboInfo?.ComboName ?? "(Combo không xác định)",
                    Quantity = orderCombo.Quantity,
                    Price = comboInfo?.Price ?? 0m
                });
            }

            ViewBag.OrderComboCount = orderCombos.Count;
            ViewBag.OrderComboRaw = orderCombos.Select(oc => new { oc.IdCombo, oc.Quantity, ComboName = oc.Combo?.ComboName ?? _context.Combos.Find(oc.IdCombo)?.ComboName }).ToList();
            ViewBag.ModelComboCount = vm.Combos.Count;

            return View(vm);
        }

        [HttpGet]
        public IActionResult Checkout(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Tickets).ThenInclude(t => t.Seat)
                .Include(o => o.Tickets).ThenInclude(t => t.Showtime).ThenInclude(s => s.Movie)
                .Include(o => o.OrderCombos).ThenInclude(oc => oc.Combo)
                .FirstOrDefault(o => o.IdOrder == orderId);

            if (order == null) return NotFound();

            if (order.Status != "Chờ thanh toán")
                return RedirectToAction("Index", "Home");

            if (order.ExpiredAt < DateTime.Now)
            {
                order.Status = "Đã hủy";
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            var firstShowtime = order.Tickets.FirstOrDefault()?.Showtime;

            var orderCombos = order.OrderCombos?.ToList() ??
                _context.OrderCombos.Include(oc => oc.Combo).Where(oc => oc.IdOrder == order.IdOrder).ToList();

            var vm = new PaymentCheckoutViewModel
            {
                OrderId = order.IdOrder,
                MovieName = firstShowtime?.Movie?.MovieName ?? "",
                Poster = firstShowtime?.Movie?.Poster ?? "",
                ShowtimeDate = firstShowtime?.StartFilm ?? DateTime.Now,
                ShowtimeTime = firstShowtime != null ? firstShowtime.StartTime.ToString("HH:mm") : "",
                TicketCount = order.Tickets.Count,
                TicketTotal = order.Tickets.Sum(t => t.FinalPrice),
                ComboTotal = orderCombos.Sum(oc => (oc.Combo?.Price ?? _context.Combos.Find(oc.IdCombo)?.Price ?? 0m) * oc.Quantity),
                Total = order.TotalPrice,
                Status = order.Status
            };

            foreach (var ticket in order.Tickets)
            {
                if (ticket.Seat != null)
                    vm.SeatNames.Add($"{ticket.Seat.SeatRow}{ticket.Seat.SeatNumber}");
            }

            foreach (var orderCombo in orderCombos)
            {
                if (orderCombo.Quantity <= 0) continue;

                var comboInfo = orderCombo.Combo ?? _context.Combos.Find(orderCombo.IdCombo);

                vm.Combos.Add(new PaymentComboItem
                {
                    IdCombo = orderCombo.IdCombo,
                    ComboName = comboInfo?.ComboName ?? "(Combo không xác định)",
                    Quantity = orderCombo.Quantity,
                    Price = comboInfo?.Price ?? 0m
                });
            }

            ViewBag.OrderComboCount = orderCombos.Count;
            ViewBag.OrderComboRaw = orderCombos.Select(oc => new { oc.IdCombo, oc.Quantity, ComboName = oc.Combo?.ComboName ?? _context.Combos.Find(oc.IdCombo)?.ComboName }).ToList();
            ViewBag.ModelComboCount = vm.Combos.Count;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, string paymentType, decimal total)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Auth");

            var order = _context.Orders.Find(orderId);
            if (order == null)
                return NotFound();

            if (order.ExpiredAt < DateTime.Now)
            {
                order.Status = "Đã hủy";
                _context.SaveChanges();

                return BadRequest("Đơn đã hết hạn");
            }

            if (order.TotalPrice != total)
                return BadRequest("Số tiền thanh toán không hợp lệ.");

            var payment = PaymentFactory.CreatePayment(paymentType);
            payment.Pay(total);

            string status;
            string orderStatus;
            string transCode;

            if (paymentType.ToLower() == "cash")
            {
                status = PaymentConstants.StatusPending;
                orderStatus = PaymentConstants.OrderPending;
                transCode = "CASH-" + DateTime.Now.Ticks;
            }
            else if (paymentType.ToLower() == "vnpay")
            {
                status = PaymentConstants.StatusPaid;
                orderStatus = PaymentConstants.OrderPaid;
                transCode = "LOCAL-VNPAY-" + DateTime.Now.Ticks;
            }
            else if (paymentType.ToLower() == "momo")
            {
                // Chế độ fake Momo local
                status = PaymentConstants.StatusPaid;
                orderStatus = PaymentConstants.OrderPaid;
                transCode = "LOCAL-MOMO-" + DateTime.Now.Ticks;
            }
            else
            {
                status = PaymentConstants.StatusPaid;
                orderStatus = PaymentConstants.OrderPaid;
                transCode = "TX" + DateTime.Now.Ticks;
            }

            var paymentRecord = new Payment
            {
                PaymentMethod = paymentType.ToLower() switch
                {
                    "cash" => PaymentConstants.Cash,
                    "momo" => PaymentConstants.Momo,
                    "vnpay" => PaymentConstants.VNPay,
                    _ => "Khác"
                },
                Price = total,
                Status = status,
                TransactionCode = transCode,
                PaymentTime = DateTime.Now,
                IdOrder = orderId
            };

            _context.Payments.Add(paymentRecord);
            order.Status = orderStatus;
            _context.SaveChanges();

            _notificationSubject.Publish($"Thanh toán vé thành công.", "success");

            if (paymentType.ToLower() == "cash")
            {
                ViewBag.Message = "Đã ghi nhận thanh toán tiền mặt: đang chờ xác nhận admin.";
                _notificationSubject.Publish($"Vé đang chờ xác nhận admin.", "warning");
                return RedirectToAction("Success", new { id = orderId });
            }

            return RedirectToAction("Success", new { id = orderId });
        }

        // Chức năng trả về và notify API Momo không còn dùng khi làm fake cục bộ, có thể xóa.

        [HttpGet]
        public IActionResult Success(int id)
        {
            var order = _context.Orders
                .Include(o => o.Payments)
                .Include(o => o.Tickets).ThenInclude(t => t.Seat)
                .Include(o => o.Tickets).ThenInclude(t => t.Showtime).ThenInclude(s => s.Movie)
                .Include(o => o.OrderCombos).ThenInclude(oc => oc.Combo)
                .FirstOrDefault(o => o.IdOrder == id);

            if (order == null)
                return NotFound();

            var firstShowtime = order.Tickets.FirstOrDefault()?.Showtime;

            var orderCombos = order.OrderCombos?.ToList() ??
                _context.OrderCombos.Include(oc => oc.Combo).Where(oc => oc.IdOrder == order.IdOrder).ToList();

            var vm = new PaymentCheckoutViewModel
            {
                OrderId = order.IdOrder,
                MovieName = firstShowtime?.Movie?.MovieName ?? "",
                Poster = firstShowtime?.Movie?.Poster ?? "",
                ShowtimeDate = firstShowtime?.StartFilm ?? DateTime.Now,
                ShowtimeTime = firstShowtime != null ? firstShowtime.StartTime.ToString("HH:mm") : "",
                TicketCount = order.Tickets.Count,
                TicketTotal = order.Tickets.Sum(t => t.FinalPrice),
                ComboTotal = orderCombos.Sum(oc => (oc.Combo?.Price ?? _context.Combos.Find(oc.IdCombo)?.Price ?? 0m) * oc.Quantity),
                Total = order.TotalPrice,
                Status = order.Status
            };

            foreach (var ticket in order.Tickets)
            {
                if (ticket.Seat != null)
                    vm.SeatNames.Add($"{ticket.Seat.SeatRow}{ticket.Seat.SeatNumber}");
            }

            foreach (var orderCombo in orderCombos)
            {
                if (orderCombo.Quantity <= 0) continue;

                var comboInfo = orderCombo.Combo ?? _context.Combos.Find(orderCombo.IdCombo);

                vm.Combos.Add(new PaymentComboItem
                {
                    IdCombo = orderCombo.IdCombo,
                    ComboName = comboInfo?.ComboName ?? "(Combo không xác định)",
                    Quantity = orderCombo.Quantity,
                    Price = comboInfo?.Price ?? 0m
                });
            }

            ViewBag.OrderComboCount = orderCombos.Count;
            ViewBag.OrderComboRaw = orderCombos.Select(oc => new { oc.IdCombo, oc.Quantity, ComboName = oc.Combo?.ComboName ?? _context.Combos.Find(oc.IdCombo)?.ComboName }).ToList();
            ViewBag.ModelComboCount = vm.Combos.Count;

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
