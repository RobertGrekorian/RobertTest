﻿namespace RobertTest.Services.AuthApi.Data.Dto
{
    public class RegisterRequestDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
