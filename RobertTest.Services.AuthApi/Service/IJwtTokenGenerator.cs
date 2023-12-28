using RobertTest.Services.AuthApi.Models;

namespace RobertTest.Services.AuthApi.Service
{
    public interface IJwtTokenGenerator
    {
        string GenerateJwtToken(ApplicationUser user,IEnumerable<string> roles);
    }
}
