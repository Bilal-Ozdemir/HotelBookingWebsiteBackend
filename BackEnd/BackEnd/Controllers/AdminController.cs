using System;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.DTOs;                              // ← import your DTOs
using BackEnd.Entities;
using BackEnd.UseCases.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext     _context;
        private readonly GenerateJwtToken _generateJwtToken;

        public AdminController(
            AppDbContext     context,
            GenerateJwtToken generateJwtToken)
        {
            _context          = context;
            _generateJwtToken = generateJwtToken;
        }

        // POST: api/admin/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email || u.Username == dto.Email);
            if (user == null)
                return Unauthorized(new { error = "Invalid credentials" });

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result != PasswordVerificationResult.Success || user.Role != "Admin")
                return Unauthorized(new { error = "Invalid credentials or not an admin" });

            var token = _generateJwtToken.Execute(user);
            return Ok(new { token });
        }

        // GET: api/admin/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStatistics()
        {
            int totalBookings  = await _context.Bookings.CountAsync();
            int totalRooms     = await _context.HotelRooms.CountAsync();
            DateTime today     = DateTime.UtcNow.Date;
            int occupied       = await _context.Bookings.CountAsync(b => b.CheckIn <= today && b.CheckOut > today);
            int occupancyRate  = totalRooms == 0 ? 0 : (int)Math.Round(occupied * 100.0 / totalRooms);
            decimal totalRev   = await _context.Payments.SumAsync(p => p.Amount);

            return Ok(new
            {
                totalBookings,
                occupancyRate,
                roomsAvailable = totalRooms - occupied,
                totalRevenue   = totalRev
            });
        }

        // GET: api/admin/bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.HotelRoom).ThenInclude(r => r.RoomTypes)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .ToListAsync();

            return Ok(bookings);
        }

        // PUT & DELETE for bookings omitted for brevity…

        // ========================
        // User management section
        // ========================

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto {
                    Id       = u.Id,
                    Username = u.Username,   // ← your actual property name
                    Email    = u.Email
                })
                .ToListAsync();

            return Ok(users);
        }

        // PUT: api/admin/users/{id}
        [HttpPut("users/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { error = "Username and email are required." });

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { error = "User not found." });

            user.Username = dto.Username;
            user.Email    = dto.Email;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { error = $"Could not update user: {ex.Message}" });
            }

            return NoContent();
        }

        // DELETE: api/admin/users/{id}
        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { error = "User not found." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
