using Microsoft.AspNetCore.Identity;

namespace RobertTest.Services.AuthApi.Models
{
    public class ApplicationUser : IdentityUser 
    {
        public string Name { get; set; }
    }
}
