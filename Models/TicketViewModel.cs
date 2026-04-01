public class TicketViewModel
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public string MovieName { get; set; }
    public DateTime WatchDate { get; set; }
    public string Showtime { get; set; }
    public string Seats { get; set; } // Ví dụ: "A1, A2"
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public DateTime OrderTime { get; set; }
}