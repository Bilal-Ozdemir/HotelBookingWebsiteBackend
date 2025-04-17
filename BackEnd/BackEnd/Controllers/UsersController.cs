using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context)
            => _context = context;

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            // ensure the user is only fetching their own profile
            var currentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (currentId != id)
                return Forbid();

            var u = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (u == null) 
                return NotFound();

            return Ok(new UserDto {
                Id       = u.Id,
                Username = u.Username,
                Email    = u.Email
            });
        }

        // PUT: api/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            // make sure they can only update their own profile
            var currentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (currentId != id)
                return Forbid();

            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { error = "Username and email are required." });

            var u = await _context.Users.FindAsync(id);
            if (u == null)
                return NotFound(new { error = "User not found." });

            u.Username = dto.Username;
            u.Email    = dto.Email;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateException ex) {
                return BadRequest(new { error = $"Could not update user: {ex.Message}" });
            }

            return NoContent();
        }
    }
}
