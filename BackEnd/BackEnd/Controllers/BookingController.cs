using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BackEnd.DTOs;
using BackEnd.UseCases.Bookings;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookingController : ControllerBase
    {
        private readonly CreateBooking _createBooking;

        public BookingController(CreateBooking createBooking)
        {
            _createBooking = createBooking;
        }

        [HttpPost]
        public async Task<IActionResult> BookRoom([FromBody] BookingRequestDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid token.");

            try
            {
                var booking = await _createBooking.Execute(userId, dto.RoomTypeId, dto.CheckIn, dto.CheckOut);
                return Ok(new { message = "Booking successful!", bookingId = booking.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
