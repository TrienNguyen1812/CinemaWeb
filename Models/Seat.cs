using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class Seat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSeat { get; set; }

        [Required, Column(TypeName = "nvarchar(10)")]
        public string SeatRow { get; set; }

        [Required, Column(TypeName = "nvarchar(10)")]
        public string SeatNumber { get; set; }

        [Required, Column(TypeName = "nvarchar(10)")]
        public string TypeSeat { get; set; }

        [Required]
        public int IdCinema { get; set; }
        public Cinema? Cinema { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
    }
}
