using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RobertTest.Services.AuthApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RobertTest.Services.AuthApi.Service
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _config;
        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        

        public string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("ApiSettings:JwtOptions:Secret"));

            var secureKey = new SymmetricSecurityKey(key);

            var claims = new List<Claim>();

            var claim1 = new Claim("Name",user.Email);
            var claim2 = new Claim("Email", user.Email);
            var claim3 = new Claim("Sub", user.Id);

            claims.Add(claim1);
            claims.Add(claim2);
            claims.Add(claim3);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role,role)));

            var tokenDescriptor = new SecurityTokenDescriptor();
            tokenDescriptor.Issuer = _config.GetValue<string>("ApiSettings:JwtOptions:Issuer");
            tokenDescriptor.Audience = _config.GetValue<string>("ApiSettings:JwtOptions:Audience");
            tokenDescriptor.Subject = new ClaimsIdentity(claims);
            tokenDescriptor.Expires = DateTime.Now.AddHours(12);
            tokenDescriptor.SigningCredentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha256Signature);

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); 
        }
    }
}
