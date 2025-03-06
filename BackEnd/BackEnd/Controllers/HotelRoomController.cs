using BackEnd.BackEnd.Data;
using BackEnd.BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelRoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HotelRoomController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/hotelroom
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelRoom>>> GetHotelRooms()
        {
            return await _context.HotelRooms.Include(hr => hr.RoomTypes).ToListAsync();
        }

        // GET: api/hotelroom/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelRoom>> GetHotelRoom(int id)
        {
            var hotelRoom = await _context.HotelRooms
                .Include(hr => hr.RoomTypes) // Include RoomType details if necessary
                .FirstOrDefaultAsync(hr => hr.Id == id);

            if (hotelRoom == null)
            {
                return NotFound();
            }

            return Ok(hotelRoom);
        }

        // POST: api/hotelroom
        [HttpPost]
        public async Task<ActionResult<HotelRoom>> CreateHotelRoom([FromBody] HotelRoom hotelRoom)
        {
            _context.HotelRooms.Add(hotelRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotelRoom), new { id = hotelRoom.Id }, hotelRoom);
        }

        // PUT: api/hotelroom/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotelRoom(int id, [FromBody] HotelRoom updatedHotelRoom)
        {
            if (id != updatedHotelRoom.Id)
            {
                return BadRequest();
            }

            _context.Entry(updatedHotelRoom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelRoomExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/hotelroom/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelRoom(int id)
        {
            var hotelRoom = await _context.HotelRooms.FindAsync(id);
            if (hotelRoom == null)
            {
                return NotFound();
            }

            _context.HotelRooms.Remove(hotelRoom);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HotelRoomExists(int id)
        {
            return _context.HotelRooms.Any(e => e.Id == id);
        }
    }
}
