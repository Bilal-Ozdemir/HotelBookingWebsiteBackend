using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.BackEnd.Data;
using BackEnd.BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto registerDto)
    {
        // Check if the email is already registered
        if (_context.Users.Any(u => u.Email == registerDto.Email))
        {
            return BadRequest(new { message = "Email already exists." });
        }

        // Hash the password before storing
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Create new user
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = hashedPassword
        };

        // Save user to the database
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new { message = "User registered successfully!" });
    }

    /// <summary>
    /// User login and token generation
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        // Find the user by email
        var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);

        // Validate user existence and password match
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Generate JWT Token
        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    /// <summary>
    /// Generate JWT token for authentication
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// DTO for user registration
/// </summary>
public class RegisterDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

/// <summary>
/// DTO for user login
/// </summary>
public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}


