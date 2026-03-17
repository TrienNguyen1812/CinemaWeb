using System.ComponentModel.DataAnnotations;

namespace CinemaWeb.Models
{
    public class Combo
    {
        [Key]
        public int IdCombo { get; set; }

        [Required]
        public string ComboName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public ICollection<OrderCombo>? OrderCombos { get; set; }
    }
}
