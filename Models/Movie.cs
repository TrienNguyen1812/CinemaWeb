using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMovie { get; set; }

        [Required, Column(TypeName = "nvarchar(255)")]
        public string MovieName { get; set; }

        [Required, Column(TypeName = "nvarchar(30)")]
        public string Category { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required, Column(TypeName = "date")]
        public DateTime ReleaseDate { get; set; }

        [Required, Column(TypeName = "nvarchar(255)")]
        public string Description { get; set; }

        [Required, Column(TypeName = "varchar(30)")]
        public string Poster { get; set; }

        [Required, Column(TypeName = "varchar(30)")]
        public string Trailer { get; set; }

        [Required, Column(TypeName = "nvarchar(30)")]
        public string Age { get; set; }

        public ICollection<Showtime>? Showtimes { get; set; }
    }
}
