using CinemaWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class ScreeningRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRoom { get; set; }

        [Required]
        public string RoomName { get; set; }

        public int SeatQuantity { get; set; }

        public int IdCinema { get; set; }

        [ForeignKey("IdCinema")]
        public Cinema? Cinema { get; set; }

        public ICollection<Showtime>? Showtimes { get; set; }
        public ICollection<Seat>? Seats { get; set; }
    }
}
