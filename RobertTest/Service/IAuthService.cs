using RobertTest.Models.Dto;

namespace RobertTest.Service
{
    public interface IAuthService
    {
        Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);

        Task<ResponseDto> AssignRoleAsync(RegisterRequestDto registerRequestDto);

    }
}
