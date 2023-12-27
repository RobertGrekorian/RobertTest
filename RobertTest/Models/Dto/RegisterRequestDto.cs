using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RobertTest.Models.Dto
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }       
        public string? Role { get; set; }
    }
}
