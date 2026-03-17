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
            var order = new Order
            {
                OrderTime = DateTime.Now,
                Status = "Pending",
                IdUser = command.IdUser
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var seatId in command.SeatIds)
            {
                var show = _context.Showtimes
                    .First(x => x.IdShowtime == command.IdShowtime);

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

            foreach (var combo in command.Combos)
            {
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
