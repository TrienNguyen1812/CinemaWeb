using CinemaWeb.Services;
using Microsoft.AspNetCore.Mvc;
using CinemaWeb.Models;

namespace CinemaWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        public AdminController(IOrderRepository orderRepository, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }
        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalRevenue = _orderRepository.GetTotalRevenue(),
                TotalOrders = _orderRepository.CountCompletedOrders(),
                TotalCustomers = _customerRepository.CountActiveCustomers(),
                DailySales = _orderRepository.CountTodaySales(),

                ChartLabels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                RevenueData = _orderRepository.GetRevenueByMonth(),
                OrderData = _orderRepository.GetOrdersByMonth(),

                // Lấy dữ liệu thật ở đây
                TopCustomers = _customerRepository.GetTopCustomers(3), 
                RecentOrders = _orderRepository.GetRecentOrders(5)
            };

            return View(model);
        }
    }
}
