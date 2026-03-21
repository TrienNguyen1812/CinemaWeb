// Models/DashboardViewModel.cs
namespace CinemaWeb.Models
{ 
    public class DashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int DailySales { get; set; }
        
        public List<string> ChartLabels { get; set; }
        public List<decimal> RevenueData { get; set; }
        public List<int> OrderData { get; set; }

        // Đổi tên từ TopSellers thành TopCustomers (Những người mua vé nhiều nhất)
        public List<TopCustomerDto> TopCustomers { get; set; }

        // Đổi tên từ LatestProjects thành RecentOrders (Các đơn đặt vé mới nhất)
        public List<RecentOrderDto> RecentOrders { get; set; }
    }
        public class TopCustomerDto
    {
        public string CustomerName { get; set; }
        public decimal TotalSpent { get; set; }
        public int TicketCount { get; set; }
    }

    public class RecentOrderDto
    {
        public int OrderId { get; set; }
        public string MovieName { get; set; } // Tên phim khách đặt
        public DateTime OrderTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // Ví dụ: Đã thanh toán, Chờ xử lý...
    }
}
