using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.UseCases.Auth;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GenerateJwtToken _generateJwtToken;

        public AdminController(AppDbContext context, GenerateJwtToken generateJwtToken)
        {
            _context = context;
            _generateJwtToken = generateJwtToken;
        }

        // GET: api/admin/bookings
        [HttpGet("bookings")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.HotelRoom)
                    .ThenInclude(hr => hr.RoomTypes)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .ToListAsync();

            return Ok(bookings);
        }

        // GET: api/admin/bookings/{id}
        [HttpGet("bookings/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.HotelRoom)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        // PUT: api/admin/bookings/{id}
        [HttpPut("bookings/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking updatedBooking)
        {
            if (id != updatedBooking.Id)
                return BadRequest("Booking ID mismatch.");

            var room = await _context.HotelRooms.FindAsync(updatedBooking.RoomId);
            if (room == null)
                return BadRequest("Room not found.");

            var overlapping = await _context.Bookings
                .AnyAsync(b => b.RoomId == updatedBooking.RoomId &&
                               b.Id != id &&
                               ((updatedBooking.CheckIn >= b.CheckIn && updatedBooking.CheckIn < b.CheckOut) ||
                                (updatedBooking.CheckOut > b.CheckIn && updatedBooking.CheckOut <= b.CheckOut)));

            if (overlapping)
                return BadRequest("Room is already booked for the selected dates.");

            _context.Entry(updatedBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/admin/bookings/{id}
        [HttpDelete("bookings/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/admin/stats
        [HttpGet("stats")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetStatistics()
        {
            int totalBookings = await _context.Bookings.CountAsync();
            int totalRooms = await _context.HotelRooms.CountAsync();
            DateTime today = DateTime.UtcNow.Date;
            int occupiedRooms = await _context.Bookings.CountAsync(b => b.CheckIn <= today && b.CheckOut > today);
            int occupancyPercent = totalRooms == 0 ? 0 : (int)Math.Round(occupiedRooms * 100.0 / totalRooms);
            decimal totalRevenue = await _context.Payments.SumAsync(p => p.Amount);

            return Ok(new
            {
                totalBookings,
                occupancyRate = occupancyPercent,
                roomsAvailable = totalRooms - occupiedRooms,
                totalRevenue
            });
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
            if (result != PasswordVerificationResult.Success)
                return Unauthorized(new { error = "Invalid credentials" });

            if (user.Role != "Admin")
                return Unauthorized(new { error = "Access denied: Admins only" });

            var token = _generateJwtToken.Execute(user);
            return Ok(new { token });
        }

        private bool BookingExists(int id) => _context.Bookings.Any(e => e.Id == id);
    }
}
