using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using BackEnd.Entities;

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
    /// Get all bookings for admin use
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
    {
        return await _context.Bookings
            .Include(b => b.HotelRoom)
            .Include(b => b.User)
            .ToListAsync();
    }

    /// <summary>
    /// Get a single booking by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.HotelRoom)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        return booking;
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

    /// <summary>
    /// Create a new booking (Ensures proper room availability check)
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized("User not logged in.");

        var room = await _context.HotelRooms.FindAsync(booking.RoomId);
        if (room == null)
        {
            return BadRequest("Room not found.");
        }

        // Check if the room is already booked for the given dates
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
        await _context.SaveChangesAsync();

        return Ok(new { message = "Booking successful!", bookingId = booking.Id });
    }

    /// <summary>
    /// Delete a booking
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
