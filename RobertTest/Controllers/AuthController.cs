using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RobertTest.Models.Dto;
using RobertTest.Service;
using RobertTest.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace RobertTest.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }
        [HttpGet]

        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();

            return View(loginRequestDto);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
           
            ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                _tokenProvider.SetToken(loginResponseDto.Token);
                await SignInUser(loginResponseDto);
                TempData["success"] = "Login Successful";
                return RedirectToAction(nameof(Customers),"Customers");
                
            }
            ModelState.AddModelError("CustomError", responseDto.Message) ;
            TempData["error"] = "UserName Or Password Not Found!";
            return View(loginRequestDto);
        }
        [HttpGet]
        public IActionResult Register()
        {
            var rolelist = new List<SelectListItem>()
            {
                new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer },
            };
            ViewBag.RoleList = rolelist;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            if (string.IsNullOrEmpty(registerRequestDto.Role))
            {
                registerRequestDto.Role = SD.RoleCustomer;
            }
            ResponseDto responseDto = await _authService.RegisterAsync(registerRequestDto);
            ResponseDto assignDto;
            if (responseDto != null && responseDto.IsSuccess)
            {
                if(string.IsNullOrEmpty(registerRequestDto.Role)) 
                {
                    registerRequestDto.Role = SD.RoleCustomer;
                }
                assignDto = await _authService.AssignRoleAsync(registerRequestDto);
                if(assignDto != null && assignDto.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            var rolelist = new List<SelectListItem>()
            {
                new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer },
            };
            ViewBag.RoleList = rolelist;
            TempData["error"] = responseDto.Message;
            return View(registerRequestDto);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index","Home");
        }
        private async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            var claim1 = new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u=> u.Type == "Name").Value);
            var claim2 = new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == "Email").Value);
            var claim3 = new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == "Sub").Value);
            var claim4 = new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "Email").Value);
            var claim5 = new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value);
            identity.AddClaim(claim1);
            identity.AddClaim(claim2);
            identity.AddClaim(claim3);
            identity.AddClaim(claim4);
            identity.AddClaim(claim5);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        }
    }
}
