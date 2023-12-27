using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RobertTest.Services.AuthApi.Data.Dto;
using RobertTest.Services.AuthApi.Service;

namespace RobertTest.Services.AuthApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        public AuthApiController(IAuthService authService)
        {
            _responseDto = new ResponseDto();
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var errorMessage = await _authService.Register(registerRequestDto);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            LoginResponseDto loginResponseDto = await _authService.Login(loginRequestDto);
            if (string.IsNullOrEmpty(loginResponseDto.Token) || loginResponseDto.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Password or Username was wrong";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponseDto;
            return Ok(_responseDto);
        }
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterRequestDto registerRequestDto)
        {
            var result = await _authService.AssignRole(registerRequestDto.Email,registerRequestDto.Role.ToUpper());
            if (result == false)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Something went wrong while assigning the role";
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }

    }
}
