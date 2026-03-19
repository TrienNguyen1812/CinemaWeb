using CinemaWeb.Models;

namespace CinemaWeb.Services.Commands
{
    public class CreateOrderCommandHandler
    {
        private readonly DbContexts _context;

        public CreateOrderCommandHandler(DbContexts context)
        {
            _context = context;
        }

        public int Handle(CreateOrderCommand command)
        {
            var show = _context.Showtimes
                .First(x => x.IdShowtime == command.IdShowtime);

            var ticketTotal = (command.SeatIds?.Count ?? 0) * show.Price;
            var comboTotal = 0m;

            if (command.Combos != null)
            {
                foreach (var combo in command.Combos)
                {
                    if (combo.Value <= 0) continue;
                    var comboEntity = _context.Combos.Find(combo.Key);
                    if (comboEntity != null)
                        comboTotal += comboEntity.Price * combo.Value;
                }
            }

            var order = new Order
            {
                OrderTime = DateTime.Now,
                Status = "Pending",
                IdUser = command.IdUser,
                TotalPrice = ticketTotal + comboTotal
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var seatId in command.SeatIds ?? new List<int>())
            {
                var ticket = new Ticket
                {
                    IdSeat = seatId,
                    IdShowtime = command.IdShowtime,
                    IdOrder = order.IdOrder,
                    OriginalPrice = show.Price,
                    FinalPrice = show.Price
                };

                _context.Tickets.Add(ticket);
            }

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

            _context.SaveChanges();

            return order.IdOrder;
        }
    }
}
