using System.ComponentModel.DataAnnotations;

namespace CinemaWeb.Models
{
    public class OrderCombo
    {
        [Key]
        public int IdOrderCombo { get; set; }

        public int Quantity { get; set; }

        public int IdOrder { get; set; }
        public Order? Order { get; set; }

        public int IdCombo { get; set; }
        public Combo? Combo { get; set; }
    }
}
