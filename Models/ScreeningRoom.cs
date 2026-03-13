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

        [Required, Column(TypeName = "nvarchar(255)")]
        public string RoomName { get; set; }

        [Required]
        public int SeatQuantity { get; set; }

        [Required,ForeignKey("IdCinema")]
        public int IdCinema { get; set; }
        public Cinema? Cinema { get; set; }

        public ICollection<Showtime>? Showtimes { get; set; }
    }
}
