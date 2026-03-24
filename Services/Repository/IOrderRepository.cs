using CinemaWeb.Models;

public interface IOrderRepository
{
    decimal GetTotalRevenue();
    int CountCompletedOrders();
    int CountTodaySales();
    List<decimal> GetRevenueByMonth();
    List<int> GetOrdersByMonth();
    List<string> GetChartLabels();
    List<RecentOrderDto> GetRecentOrders(int count);
}