using JwtDemoProject.Dtos;
using JwtDemoProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtDemoProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public static User user = new User();
       

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            CreatePasswordHasher(userDto.Password, out byte[] hash, out byte[] salt);
            user.Username= userDto.Username;
            user.PassordHash=hash;
            user.PasswordSalt=salt;
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto userDto)
        {
            if (user.Username != userDto.Username)
            {
                return BadRequest("User not found");
            }
            string token = CreateToken(user);
            return token;
        }
        private void CreatePasswordHasher(string password, out byte[] hash, out byte[] salt)
        {
            using(var hmac = new HMACSHA512())
            {
                hash = hmac.Key;
                salt = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims:claims,
                expires:DateTime.Now.AddMinutes(20),
                signingCredentials:creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
