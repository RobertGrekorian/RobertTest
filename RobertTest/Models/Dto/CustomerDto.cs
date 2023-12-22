using System.ComponentModel.DataAnnotations;

namespace RobertTest.Models.Dto
{
    public class CustomerDto
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public IFormFile image { get; set; }
    }
}
