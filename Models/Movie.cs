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

        // --- THÊM TRƯỜNG NÀY ---
        [Required, Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; } = MovieStatus.Showing; // Mặc định là đang chiếu

        public ICollection<Showtime>? Showtimes { get; set; }
    }

    // Tạo một class hằng số để quản lý cho dễ
    public static class MovieStatus
    {
        public const string Showing = "Đang chiếu";
        public const string ComingSoon = "Sắp chiếu";
        public const string Ended = "Ngừng chiếu";
    }
}