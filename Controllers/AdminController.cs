using CinemaWeb.Services;
using Microsoft.AspNetCore.Mvc;
using CinemaWeb.Models;

namespace CinemaWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMovieRepository _movieRepository;

        public AdminController(IOrderRepository orderRepository, ICustomerRepository customerRepository, IMovieRepository movieRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _movieRepository = movieRepository;
        }
        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalRevenue = _orderRepository.GetTotalRevenue(),
                RevenueGrowth = _orderRepository.GetRevenueGrowth(),
                TotalOrders = _orderRepository.CountCompletedOrders(),
                TotalCustomers = _customerRepository.CountActiveCustomers(),
                DailySales = _orderRepository.CountTodaySales(),

                ChartLabels = _orderRepository.GetChartLabels(),
                RevenueData = _orderRepository.GetRevenueByMonth(),
                OrderData = _orderRepository.GetOrdersByMonth(),

                // Lấy dữ liệu thật ở đây
                TopMovies = _movieRepository.GetTopMovies(3),
                TopCustomers = _customerRepository.GetTopCustomers(3), 
                RecentOrders = _orderRepository.GetRecentOrders(5)
            };

            return View(model);
        }
    }
}
