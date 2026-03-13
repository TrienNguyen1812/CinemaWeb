using CinemaWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class Showtime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdShowtime { get; set; }

        [Required, Column(TypeName = "date")]
        public DateTime StartFilm { get; set; }

        [Required, Column(TypeName = "date")]
        public DateTime StartTime { get; set; }

        [Required, Column(TypeName = "numeric(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int IdMovie { get; set; }
        public Movie? Movie { get; set; }

        [Required]
        public int IdRoom { get; set; }
        public ScreeningRoom? ScreeningRoom { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
    }
}
