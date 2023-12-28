using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RobertTest.Services.AuthApi.Data;
using RobertTest.Services.AuthApi.Data.Dto;
using RobertTest.Services.AuthApi.Models;

namespace RobertTest.Services.AuthApi.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        

        public async Task<LoginResponseDto>Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u=> u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool IsValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if(IsValid == false || user == null)
            {
                return new LoginResponseDto() { User = null, Token = ""};                
            }

            // if user found
            UserDto userDto = new()
            {
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                ID = user.Id,                
            };
            var roles = await _userManager.GetRolesAsync(user);
            var Token = _jwtTokenGenerator.GenerateJwtToken(user,roles);
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = Token
            };


            return loginResponseDto;

        }

        public async Task<string> Register(RegisterRequestDto registerRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
                NormalizedEmail = registerRequestDto.Email.ToUpper(),
                Name = registerRequestDto.Name,
                PhoneNumber = registerRequestDto.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user,registerRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = await _db.ApplicationUsers.FirstOrDefaultAsync(x=>x.UserName == registerRequestDto.Email);
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        ID = userToReturn.Id,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;

                }
            }
            catch (Exception ex)
            {
               return ex.Message;
            }
            return "Error Encountered";
        }

        public async Task<bool> AssignRole(string email, string role)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == email);
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, role);
                return true;
            }
            return false;
        }
    }
}
