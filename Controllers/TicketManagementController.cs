using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CinemaWeb.Models;

public class TicketManagementController : Controller
{
    private readonly DbContexts _context;

    public TicketManagementController(DbContexts context)
    {
        _context = context;
    }

    public IActionResult Index(string searchString)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.Tickets).ThenInclude(t => t.Seat)
            .Include(o => o.Tickets).ThenInclude(t => t.Showtime).ThenInclude(s => s.Movie)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(o => o.IdOrder.ToString() == searchString || o.User.FullName.Contains(searchString));
        }

        var result = query.OrderByDescending(o => o.OrderTime)
            .Select(o => new TicketViewModel
            {
                OrderId = o.IdOrder, // Map IdOrder vào OrderId của ViewModel
                CustomerName = o.User.FullName,
                MovieName = o.Tickets.FirstOrDefault().Showtime.Movie.MovieName,
                WatchDate = o.Tickets.FirstOrDefault().WatchDate,
                Showtime = o.Tickets.FirstOrDefault().Showtime.StartTime.ToString(@"hh\:mm"),
                // Lấy SeatRow và SeatNumber từ danh sách Tickets
                Seats = string.Join(", ", o.Tickets.Select(t => t.Seat.SeatRow + t.Seat.SeatNumber)),
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                OrderTime = o.OrderTime
            }).ToList();

        return View(result);
    }

    // Trang chi tiết vé cho Admin
    public IActionResult Details(int id)
    {
        var order = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderCombos).ThenInclude(oc => oc.Combo)
            .Include(o => o.Tickets).ThenInclude(t => t.Seat)
            .Include(o => o.Tickets).ThenInclude(t => t.Showtime).ThenInclude(s => s.Movie)
            .Include(o => o.Payments) // Xem lịch sử thanh toán nếu có
            .FirstOrDefault(o => o.IdOrder == id);

        if (order == null) return NotFound();

        return View(order);
    }
}