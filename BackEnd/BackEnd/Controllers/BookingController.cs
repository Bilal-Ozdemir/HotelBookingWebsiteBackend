// BackEnd/Controllers/BookingController.cs
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BackEnd.DTOs;
using BackEnd.UseCases.Bookings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookingController : ControllerBase
    {
        private readonly CreateBooking  _createBooking;
        private readonly GetBooking     _getBooking;
        private readonly GetMyBookings  _getMyBookings;
        private readonly DeleteBooking  _deleteBooking;

        public BookingController(
            CreateBooking  createBooking,
            GetBooking     getBooking,
            GetMyBookings  getMyBookings,
            DeleteBooking  deleteBooking)
        {
            _createBooking  = createBooking;
            _getBooking     = getBooking;
            _getMyBookings  = getMyBookings;
            _deleteBooking  = deleteBooking;
        }

        // Helper to fetch & parse the user ID from JWT
        private int GetCurrentUserId()
        {
            var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(raw, out var id))
                throw new UnauthorizedAccessException("Invalid token.");
            return id;
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<IActionResult> BookRoom([FromBody] BookingRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var booking = await _createBooking.Execute(
                    userId, dto.RoomTypeId, dto.CheckIn, dto.CheckOut
                );
                return Ok(new
                {
                    message   = "Booking successful!",
                    bookingId = booking.Id
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid token.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/bookings/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId  = GetCurrentUserId();
                var booking = await _getBooking.Execute(id);
                if (booking == null) 
                    return NotFound("Booking not found.");
                if (booking.UserId != userId) 
                    return Forbid();
                return Ok(booking);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid token.");
            }
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            try
            {
                var userId   = GetCurrentUserId();
                var bookings = await _getMyBookings.Execute(userId);
                return Ok(bookings);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid token.");
            }
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var userId  = GetCurrentUserId();
                // Ownership check is inside DeleteBooking.Execute
                var deleted = await _deleteBooking.Execute(id, userId);
                if (!deleted)
                    return NotFound("Booking not found or not yours.");
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid token.");
            }
        }
    }
}
