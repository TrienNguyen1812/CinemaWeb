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
        [ForeignKey("ScreeningRoom")]
        public int IdRoom { get; set; }

        public ScreeningRoom? ScreeningRoom { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
    }
}
