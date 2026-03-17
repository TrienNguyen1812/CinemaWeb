using CinemaWeb.Models;

namespace CinemaWeb.Services.Commands
{
    public class BookTicketCommand
    {
        private readonly DbContexts _context;

        public BookTicketCommand(DbContexts context)
        {
            _context = context;
        }

        public void Execute(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
    }
}
