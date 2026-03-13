using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTicket { get; set; }

        [Required, Column(TypeName = "numeric(10,2)")]
        public decimal OriginalPrice { get; set; }

        [Required, Column(TypeName = "numeric(10,2)")]
        public decimal FinalPrice { get; set; }

        [Required]
        public int IdShowtime { get; set; }
        public Showtime? Showtime { get; set; }

        [Required]
        public int IdSeat { get; set; }
        public Seat? Seat { get; set; }

        [Required]
        public int IdOrder { get; set; }
        public Order? Order { get; set; }
    }
}
