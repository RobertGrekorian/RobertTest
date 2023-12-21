using System.ComponentModel.DataAnnotations;

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
    }
}
