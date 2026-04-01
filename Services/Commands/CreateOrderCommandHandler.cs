using CinemaWeb.Models;
using CinemaWeb.Services.Strategies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CinemaWeb.Services.Commands
{
    public class CreateOrderCommandHandler
    {
        private readonly DbContexts _context;
        private readonly TicketPriceService _ticketPriceService;

        public CreateOrderCommandHandler(DbContexts context, TicketPriceService ticketPriceService)
        {
            _context = context;
            _ticketPriceService = ticketPriceService;
        }

        public int Handle(CreateOrderCommand command)
        {
            // 1. Lấy thông tin suất chiếu để lấy giá vé
            var show = _context.Showtimes
                .Include(s => s.Movie)
                .FirstOrDefault(x => x.IdShowtime == command.IdShowtime);

            //Kiểm tra suất chiếu tồn tại
            DateTime showDateTime = command.WatchDate.Date.Add(show.StartTime);

            // Kiểm tra nếu thời điểm hiện tại đã vượt quá thời gian chiếu
            if (showDateTime <= DateTime.Now)
            {
                throw new Exception($"Suất chiếu phim {show.Movie?.MovieName} lúc {show.StartTime} ngày {command.WatchDate:dd/MM/yyyy} đã bắt đầu hoặc đã kết thúc. Vui lòng chọn suất khác!");
            }

            if (show == null) throw new Exception("Suất chiếu không tồn tại");

            // 2. Tạo mới đơn hàng (Order)
            var order = new Order
            {
                OrderTime = DateTime.Now,
                ExpiredAt = DateTime.Now.AddMinutes(15), 
                Status = PaymentConstants.OrderDelay,
                IdUser = command.IdUser,
                TotalPrice = 0
            };

            _context.Orders.Add(order);
            _context.SaveChanges(); // Lưu để lấy IdOrder vừa tạo tự động

            decimal runningTotal = 0m;

            // 3. Tạo các vé (Tickets) và GÁN WATCHDATE
            foreach (var seatId in command.SeatIds ?? new List<int>())
            {
                var seat = _context.Seats.Find(seatId);
                if (seat == null) continue;

                IPricingStrategy pricingStrategy = new NormalPricing();
                if (command.WatchDate.DayOfWeek == DayOfWeek.Wednesday)
                {
                    pricingStrategy = new WednesdayPricing();
                }
                else 
                {
                    // 2. Nếu không phải Thứ Tư, áp dụng logic cũ
                    if (seat.TypeSeat.Trim() == "VIP") 
                        pricingStrategy = new VipPricing();
                    else if (show.StartTime.Hours >= 22 || show.StartTime.Hours < 5) 
                        pricingStrategy = new NightPricing();
                    else 
                        pricingStrategy = new NormalPricing();
                }

                decimal finalTicketPrice = _ticketPriceService.CalculatePrice(show.Price, pricingStrategy);
                runningTotal += finalTicketPrice;

                var ticket = new Ticket
                {
                    IdSeat = seatId,
                    IdShowtime = command.IdShowtime,
                    IdOrder = order.IdOrder,
                    OriginalPrice = show.Price,
                    FinalPrice = finalTicketPrice,
                    // QUAN TRỌNG: Lưu ngày chiếu mà khách đã chọn vào đây
                    WatchDate = command.WatchDate 
                };

                _context.Tickets.Add(ticket);
            }

            // 4. Lưu các Combo đã đặt
            foreach (var combo in command.Combos ?? new Dictionary<int, int>())
            {
                if (combo.Value <= 0) continue;

                var orderCombo = new OrderCombo
                {
                    IdOrder = order.IdOrder,
                    IdCombo = combo.Key,
                    Quantity = combo.Value
                };

                _context.OrderCombos.Add(orderCombo);
            }

            // 5. Lưu tất cả Tickets và OrderCombos vào DB
            _context.SaveChanges();

            return order.IdOrder;
        }
    }
}