using BackEnd.BackEnd.Data;
using BackEnd.BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.HotelRoom)
                .Include(b => b.User)
                .ToListAsync();
        }

        // GET: api/booking/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.HotelRoom)
                .Include(b => b.User) // Include User if needed
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // POST: api/booking
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking([FromBody] Booking booking)
        {
            // Check if the room exists
            var room = await _context.HotelRooms.FindAsync(booking.RoomId);
            if (room == null)
            {
                return BadRequest("Room not found.");
            }

            // Check for overlapping bookings
            var overlappingBooking = await _context.Bookings
                .AnyAsync(b => b.RoomId == booking.RoomId &&
                               (
                                   (booking.CheckIn >= b.CheckIn && booking.CheckIn < b.CheckOut) ||
                                   (booking.CheckOut > b.CheckIn && booking.CheckOut <= b.CheckOut)
                               ));

            if (overlappingBooking)
            {
                return BadRequest("Room is already booked for the selected dates.");
            }

            // Create the booking
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        // DELETE: api/booking/{id}
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
}
