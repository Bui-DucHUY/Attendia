using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Attendia.Models;
using Attendia.Repositories;

namespace Attendia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IInstructorRepository _instructorRepo;
        private readonly IConfiguration _config;

        public AuthController(IInstructorRepository instructorRepo, IConfiguration config)
        {
            _instructorRepo = instructorRepo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Instructor request)
        {
            // Check if email already exists
            var existingUser = await _instructorRepo.GetInstructorByEmailAsync(request.InstructorMail);
            if (existingUser != null)
                return BadRequest("Email is already registered.");

            // Hash the password before saving
            request.InstructorPassword = BCrypt.Net.BCrypt.HashPassword(request.InstructorPassword);

            try
            {
                var newId = await _instructorRepo.RegisterInstructorAsync(request);
                return Ok(new { Message = "Registration successful", InstructorID = newId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _instructorRepo.GetInstructorByEmailAsync(request.Email);

            // Verify user exists and password hashes match
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.InstructorPassword))
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, InstructorName = user.InstructorName });
        }

        private string GenerateJwtToken(Instructor user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.InstructorID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.InstructorMail),
                new Claim(ClaimTypes.Name, user.InstructorName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}