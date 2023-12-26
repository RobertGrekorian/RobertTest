using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RobertTest.Models
{
    public class Payment
    {
        [Key]
        public int id { get; set; }
        [Required]

        [ForeignKey("FK_customer")]
        public int customerId { get; set; }
        public DateTime transactionDate { get; set; } = DateTime.Now;
        public double Amount { get; set; }
        public string SessionId { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
    }
}
