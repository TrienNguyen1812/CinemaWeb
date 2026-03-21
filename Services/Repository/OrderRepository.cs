using CinemaWeb.Models;
using Microsoft.EntityFrameworkCore;

public class OrderRepository : IOrderRepository
{
    private readonly DbContexts _context;

    public OrderRepository(DbContexts context)
    {
        _context = context;
    }

    // 1. Tổng doanh thu từ trước đến nay (Chỉ tính đơn đã thanh toán/hoàn thành)
    public decimal GetTotalRevenue()
    {
        return _context.Orders
            .Where(o => o.Status == PaymentConstants.OrderPaid) // Thay bằng chuỗi Status thật trong DB của bạn
            .Sum(o => (decimal?)o.TotalPrice) ?? 0;
    }

    // 2. Tổng số đơn hàng thành công
    public int CountCompletedOrders()
    {
        return _context.Orders.Count(o => o.Status == PaymentConstants.OrderPaid);
    }

    // 3. Số đơn hàng phát sinh trong hôm nay
    public int CountTodaySales()
    {
        var today = DateTime.Today;
        return _context.Orders
            .Count(o => o.OrderTime.Date == today);
    }

    // 4. Lấy doanh thu 6 tháng gần nhất để vẽ biểu đồ
    public List<decimal> GetRevenueByMonth()
    {
        var result = new List<decimal>();
        var now = DateTime.Today;

        for (int i = 5; i >= 0; i--)
        {
            var targetMonth = now.AddMonths(-i);
            var total = _context.Orders
                .Where(o => o.OrderTime.Month == targetMonth.Month && 
                            o.OrderTime.Year == targetMonth.Year &&
                            o.Status == PaymentConstants.OrderPaid)
                .Sum(o => (decimal?)o.TotalPrice) ?? 0;
            
            result.Add(total);
        }
        return result;
    }

    // 5. Lấy số lượng đơn hàng 6 tháng gần nhất
    public List<int> GetOrdersByMonth()
    {
        var result = new List<int>();
        var now = DateTime.Today;

        for (int i = 5; i >= 0; i--)
        {
            var targetMonth = now.AddMonths(-i);
            var count = _context.Orders
                .Count(o => o.OrderTime.Month == targetMonth.Month && 
                            o.OrderTime.Year == targetMonth.Year);
            
            result.Add(count);
        }
        return result;
    }

    public List<RecentOrderDto> GetRecentOrders(int count)
    {
        return _context.Orders
            .Include(o => o.User)
            // 1. Thêm điều kiện lọc trạng thái (Sửa "Paid" thành giá trị thật trong DB của bạn)
            .Where(o => o.Status == PaymentConstants.OrderPaid) 
            // 2. Sắp xếp đơn hàng mới nhất lên đầu
            .OrderByDescending(o => o.OrderTime) 
            .Take(count)
            .Select(o => new RecentOrderDto
            {
                OrderId = o.IdOrder,
                // Lấy tên phim từ Ticket đầu tiên (Lưu ý: Bạn cần dùng .Include hoặc Select để tránh Null)
                MovieName = o.Tickets
                    .Select(t => t.Showtime.Movie.MovieName)
                    .FirstOrDefault() ?? "Đồ ăn/Combo", 
                OrderTime = o.OrderTime,
                TotalPrice = o.TotalPrice,
                Status = o.Status
            })
            .ToList();
    }
}