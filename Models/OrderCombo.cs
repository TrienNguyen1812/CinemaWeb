using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class OrderCombo
    {
        [Key]
        public int IdOrderCombo { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("Order")]
        public int IdOrder { get; set; }
        public Order? Order { get; set; }

        [ForeignKey("Combo")]
        public int IdCombo { get; set; }
        public Combo? Combo { get; set; }
    }
}
