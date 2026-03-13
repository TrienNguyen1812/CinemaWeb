using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace CinemaWeb.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdOrder { get; set; }

        [Required, Column(TypeName = "date")]
        public DateTime OrderTime { get; set; }

        [Required, Column(TypeName = "numeric(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required, Column(TypeName = "nvarchar(255)")]
        public string Status { get; set; }

        [Required]
        public int IdUser { get; set; }
        public User? User { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}
