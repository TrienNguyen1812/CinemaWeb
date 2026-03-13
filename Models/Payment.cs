using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWeb.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPayment { get; set; }

        [Required, Column(TypeName = "nvarchar(30)")]
        public string PaymentMethod { get; set; }

        [Required, Column(TypeName = "numeric(10,2)")]
        public decimal Price { get; set; }

        [Required, Column(TypeName = "nvarchar(255)")]
        public string Status { get; set; }

        [Required, Column(TypeName = "nvarchar(30)")]
        public string TransactionCode { get; set; }

        [Required, Column(TypeName = "date")]
        public DateTime PaymentTime { get; set; }

        [Required]
        public int IdOrder { get; set; }
        public Order? Order { get; set; }
    }
}

