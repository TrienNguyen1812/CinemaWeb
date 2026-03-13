using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUser { get; set; }

        [Required, Column(TypeName = "nvarchar(255)")]
        public string FullName { get; set; }

        [Required, Column(TypeName = "nvarchar(30)")]
        public string Password { get; set; }

        [Required, Column(TypeName = "nvarchar(30)")]
        public string Email { get; set; }

        [Required, Column(TypeName = "nvarchar(11)")]
        public string PhoneNumber { get; set; }

        [Required, Column(TypeName = "nvarchar(10)")]
        public string Role { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
