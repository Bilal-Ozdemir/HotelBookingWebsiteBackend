using Microsoft.AspNetCore.Mvc;
using BackEnd.UseCases.Auth;
using BackEnd.DTOs;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _registerUser.Execute(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _loginUser.Execute(dto);
                var token = _generateJwtToken.Execute(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}