
// BackEnd/Controllers/BookingController.cs
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.UseCases.Bookings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookingController : ControllerBase
    {
        private readonly CreateBooking _createBooking;
        private readonly GetBooking    _getBooking;
        private readonly GetMyBookings _getMyBookings;
        private readonly DeleteBooking _deleteBooking;

        public BookingController(
            CreateBooking createBooking,
            GetBooking    getBooking,
            GetMyBookings getMyBookings,
            DeleteBooking deleteBooking)
        {
            _createBooking  = createBooking;
            _getBooking     = getBooking;
            _getMyBookings  = getMyBookings;
            _deleteBooking  = deleteBooking;
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<IActionResult> BookRoom([FromBody] BookingRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null
             || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            try
            {
                var booking = await _createBooking.Execute(
                    userId,
                    dto.RoomTypeId,
                    dto.CheckIn,
                    dto.CheckOut
                );
                return Ok(new
                {
                    message   = "Booking successful!",
                    bookingId = booking.Id
                });
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null
             || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            var booking = await _getBooking.Execute(id);
            if (booking == null)
                return NotFound("Booking not found.");

            if (booking.UserId != userId)
                return Forbid();

            return Ok(booking);
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null
             || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            var bookings = await _getMyBookings.Execute(userId);
            return Ok(bookings);
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null
             || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            var deleted = await _deleteBooking.Execute(id, userId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
