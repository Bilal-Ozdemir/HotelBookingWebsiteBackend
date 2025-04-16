using BackEnd.Data;
using BackEnd.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/bookings
        [HttpGet("bookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.HotelRoom)
                .Include(b => b.User) // Include user details if necessary
                .ToListAsync();

            return Ok(bookings);
        }

        // GET: api/admin/bookings/{id}
        [HttpGet("bookings/{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.HotelRoom)
                .Include(b => b.User) // Include user details if necessary
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // PUT: api/admin/bookings/{id}
        [HttpPut("bookings/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking updatedBooking)
        {
            if (id != updatedBooking.Id)
            {
                return BadRequest();
            }

            // Check if the room exists
            var room = await _context.HotelRooms.FindAsync(updatedBooking.RoomId);
            if (room == null)
            {
                return BadRequest("Room not found.");
            }

            // Check for overlapping bookings
            var overlappingBooking = await _context.Bookings
                .AnyAsync(b => b.RoomId == updatedBooking.RoomId &&
                               b.Id != id && // Exclude current booking
                               (
                                   (updatedBooking.CheckIn >= b.CheckIn && updatedBooking.CheckIn < b.CheckOut) ||
                                   (updatedBooking.CheckOut > b.CheckIn && updatedBooking.CheckOut <= b.CheckOut)
                               ));

            if (overlappingBooking)
            {
                return BadRequest("Room is already booked for the selected dates.");
            }

            _context.Entry(updatedBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/admin/bookings/{id}
        [HttpDelete("bookings/{id}")]
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

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
