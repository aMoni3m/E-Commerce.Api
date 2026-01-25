using DotNetEnv;
using E_Commerce.Api.Data;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CustomerDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO login)
        {
            var customer = _context.Customers.FirstOrDefault(u =>
                    u.Email == login.Email);

            if (customer == null)
            {
                return Unauthorized("invalid email");
            }
            bool valid = BCrypt.Net.BCrypt.Verify(login.Password, customer.Password);

            if (!valid)
            {
                return Unauthorized("invalid password");
            }

            var calims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,customer.Email),
                new Claim("customerId",customer.Id.ToString()),
            };
            Env.Load();
            var secretKey = Environment.GetEnvironmentVariable("SecretKey");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: null,
                audience: null,
                claims: calims,
                expires: DateTime.UtcNow.AddMinutes(45),
                signingCredentials: creds
                );

            var TokenKey = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { Token = TokenKey });
        }
    }
}