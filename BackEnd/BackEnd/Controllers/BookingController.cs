using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackEnd.BackEnd.Data;
using BackEnd.BackEnd.Models;
using System.Linq;
using System.Security.Claims;

[Route("api/bookings")]
[ApiController]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Create a new booking (Checks room availability properly)
    /// </summary>
    [HttpPost("create")]
    public IActionResult CreateBooking([FromBody] Booking booking)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized("User not logged in.");

       
        bool isRoomBooked = _context.Bookings.Any(b =>
            b.RoomId == booking.RoomId &&
            (
                (booking.CheckIn >= b.CheckIn && booking.CheckIn < b.CheckOut) ||  // New check-in is within existing booking
                (booking.CheckOut > b.CheckIn && booking.CheckOut <= b.CheckOut) ||  // New check-out is within existing booking
                (booking.CheckIn <= b.CheckIn && booking.CheckOut >= b.CheckOut)  // New booking fully overlaps an existing booking
            )
        );

        if (isRoomBooked)
        {
            return BadRequest(new { message = "Room is already booked for the selected dates." });
        }

        // Proceed with booking if the room is available
        booking.UserId = int.Parse(userId);
        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return Ok(new { message = "Booking successful!", bookingId = booking.Id });
    }

    /// <summary>
    /// Get all bookings for the logged-in user
    /// </summary>
    [HttpGet("my-bookings")]
    public IActionResult GetMyBookings()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized("User not logged in.");

        var bookings = _context.Bookings
            .Where(b => b.UserId == int.Parse(userId))
            .ToList();

        return Ok(bookings);
    }
}

