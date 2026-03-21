using CinemaWeb.Models;
using System.Linq;

namespace CinemaWeb.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbContexts _context;

        public CustomerRepository(DbContexts context)
        {
            _context = context;
        }

        public int CountActiveCustomers()
        {
            // Đếm tất cả người dùng (có thể lọc theo Role là 'Customer' nếu cần)
            return _context.Users.Count(); 
        }

        public List<TopCustomerDto> GetTopCustomers(int count)
        {
            return _context.Users
                .Select(u => new TopCustomerDto
                {
                    CustomerName = u.FullName, // Hoặc u.UserName tùy Model của bạn
                    TotalSpent = u.Orders.Where(o => o.Status == PaymentConstants.OrderPaid)
                                        .Sum(o => (decimal?)o.TotalPrice) ?? 0,
                    TicketCount = u.Orders.SelectMany(o => o.Tickets).Count()
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(count)
                .ToList();
        }
    }
}