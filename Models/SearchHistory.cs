using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class SearchHistory
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int IdMovie { get; set; }

        public DateTime SearchTime { get; set; }

        [ForeignKey("IdMovie")]
        public Movie Movie { get; set; }
    }
}