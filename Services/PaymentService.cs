using CinemaWeb.Models;
using CinemaWeb.Services.Factory;
using CinemaWeb.Services;
using Microsoft.EntityFrameworkCore;
public class PaymentService
{
    private readonly DbContexts _context;
    private readonly OrderPriceService _orderPriceService;
    public PaymentService(DbContexts context, OrderPriceService orderPriceService)
    {
        _context = context;
        _orderPriceService = orderPriceService;
    }

    public void ExecutePayment(int orderId, string paymentType) // Bỏ tham số decimal total
    {
        // Load đơn hàng kèm theo Tickets và Combos để Decorator có dữ liệu tính toán
        var order = _context.Orders
            .Include(o => o.Tickets)
            .Include(o => o.OrderCombos).ThenInclude(oc => oc.Combo)
            .FirstOrDefault(o => o.IdOrder == orderId);

        if (order == null) throw new Exception("Không tìm thấy đơn hàng");

        // Dùng Decorator để tính tổng tiền thực tế từ DB
        decimal finalTotal = _orderPriceService.CalculateTotal(order); 

        var strategy = PaymentFactory.CreatePayment(paymentType);
        strategy.Pay(finalTotal);

        // Lưu thông tin thanh toán
        var paymentRecord = new Payment
        {
            IdOrder = orderId,
            Price = finalTotal,
            PaymentMethod = strategy.GetDisplayName(), // Lưu "Tiền mặt" thay vì "cash"
            Status = strategy.GetPaymentStatus(),
            TransactionCode = strategy.GenerateTransactionCode(),
            PaymentTime = DateTime.Now
        };

        order.Status = strategy.GetOrderStatus();
        order.TotalPrice = finalTotal; // Cập nhật lại cho đồng bộ

        _context.Payments.Add(paymentRecord);
        _context.SaveChanges();
    }

    public PaymentCheckoutViewModel GetPaymentDetails(int orderId)
    {
        var order = _context.Orders
            .Include(o => o.Tickets).ThenInclude(t => t.Seat)
            .Include(o => o.Tickets).ThenInclude(t => t.Showtime).ThenInclude(s => s.Movie)
            .Include(o => o.OrderCombos).ThenInclude(oc => oc.Combo) // Load sẵn Combo ở đây
            .FirstOrDefault(o => o.IdOrder == orderId);

        if (order == null) return null;

        // 1. Tính toán lại tổng tiền thực tế qua Decorator
        decimal finalTotal = _orderPriceService.CalculateTotal(order);

        
        if (order.TotalPrice != finalTotal)
        {
            order.TotalPrice = finalTotal;
            _context.SaveChanges(); // Lưu ngay vào bảng Orders
        }

        var firstTicket = order.Tickets.FirstOrDefault();
        
        // 2. Tính toán các thành phần giá để hiển thị chi tiết
        decimal ticketSum = order.Tickets.Sum(t => t.FinalPrice);
        decimal comboSum = order.OrderCombos.Sum(oc => (oc.Combo?.Price ?? 0m) * oc.Quantity);

        var vm = new PaymentCheckoutViewModel
        {
            OrderId = order.IdOrder,
            MovieName = firstTicket?.Showtime?.Movie?.MovieName ?? "Không xác định",
            Poster = firstTicket?.Showtime?.Movie?.Poster ?? "",
            ShowtimeDate = firstTicket?.WatchDate ?? DateTime.Now,
            ShowtimeTime = firstTicket?.Showtime?.StartTime.ToString(@"hh\:mm") ?? "",
            TicketCount = order.Tickets.Count,
            TicketTotal = ticketSum,
            ComboTotal = comboSum,
            
            // --- SỬA TẠI ĐÂY: Dùng con số từ Decorator thay vì order.TotalPrice ---
            Total = finalTotal, 
            
            Status = order.Status,
            ExpiredAt = order.ExpiredAt
        };

        // 3. Lấy tên ghế
        foreach (var ticket in order.Tickets.Where(t => t.Seat != null))
        {
            vm.SeatNames.Add($"{ticket.Seat.SeatRow}{ticket.Seat.SeatNumber}");
        }

        // 4. Lấy thông tin Combo (Dùng dữ liệu đã Include, không Find lẻ tẻ)
        foreach (var orderCombo in order.OrderCombos.Where(oc => oc.Quantity > 0))
        {
            vm.Combos.Add(new PaymentComboItem
            {
                IdCombo = orderCombo.IdCombo,
                ComboName = orderCombo.Combo?.ComboName ?? "(Combo không xác định)",
                Quantity = orderCombo.Quantity,
                Price = orderCombo.Combo?.Price ?? 0m,
                Subtotal = (orderCombo.Combo?.Price ?? 0m) * orderCombo.Quantity
            });
        }

        return vm;
    }
}