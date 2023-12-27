using RobertTest.Services.AuthApi.Data.Dto;

namespace RobertTest.Services.AuthApi.Service
{
    public interface IAuthService
    {
        Task<string> Register(RegisterRequestDto registerRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string role);
    }
}
