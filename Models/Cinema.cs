using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class Cinema
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCinema { get; set; }

        [Required, Column(TypeName = "nvarchar(255)")]
        public string CinemaName { get; set; }

        [Required, Column(TypeName = "nvarchar(255)")]
        public string Address { get; set; }

        public ICollection<ScreeningRoom>? ScreeningRooms { get; set; }
        public ICollection<Seat>? Seats { get; set; }
    }
}
