using System;
using System.Collections.Generic;

namespace CinemaWeb.Models
{
    public class PaymentComboItem
    {
        public int IdCombo { get; set; }
        public string ComboName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class PaymentCheckoutViewModel
    {
        public int OrderId { get; set; }
        public string MovieName { get; set; }
        public string Poster { get; set; }
        public DateTime ShowtimeDate { get; set; }
        public string ShowtimeTime { get; set; }

        public List<string> SeatNames { get; set; } = new List<string>();
        public int TicketCount { get; set; }
        public decimal TicketTotal { get; set; }

        public List<PaymentComboItem> Combos { get; set; } = new List<PaymentComboItem>();
        public decimal ComboTotal { get; set; }

        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
