using Microsoft.AspNetCore.Mvc;
using BackEnd.Entities;
using BackEnd.UseCases.Auth; // Ensure this namespace is correct
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly RegisterUser _registerUser;
    private readonly LoginUser _loginUser;
    private readonly GenerateJwtToken _generateJwtToken;

    public AuthController(RegisterUser registerUser, LoginUser loginUser, GenerateJwtToken generateJwtToken)
    {
        _registerUser = registerUser;
        _loginUser = loginUser;
        _generateJwtToken = generateJwtToken;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            string message = await _registerUser.Execute(registerDto);
            return Ok(new { message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// User login and token generation
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var user = await _loginUser.Execute(loginDto);
            var token = _generateJwtToken.Execute(user);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
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