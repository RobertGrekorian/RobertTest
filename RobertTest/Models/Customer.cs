using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RobertTest.Models
{
    public class Customer
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        public string address { get; set; } 
        public string city { get; set; }
        public string image { get; set; }
        public IEnumerable<Payment> Payments { get; set; }
    }
}
