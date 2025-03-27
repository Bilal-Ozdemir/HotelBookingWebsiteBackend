using BackEnd.Entities;
using BackEnd.UseCases.HotelRooms; // Ensure the namespace matches your project structure
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelRoomController : ControllerBase
    {
        private readonly GetHotelRooms _getHotelRooms;
        private readonly GetHotelRoom _getHotelRoom;
        private readonly CreateHotelRoom _createHotelRoom;
        private readonly UpdateHotelRoom _updateHotelRoom;
        private readonly DeleteHotelRoom _deleteHotelRoom;

        public HotelRoomController(
            GetHotelRooms getHotelRooms,
            GetHotelRoom getHotelRoom,
            CreateHotelRoom createHotelRoom,
            UpdateHotelRoom updateHotelRoom,
            DeleteHotelRoom deleteHotelRoom)
        {
            _getHotelRooms = getHotelRooms;
            _getHotelRoom = getHotelRoom;
            _createHotelRoom = createHotelRoom;
            _updateHotelRoom = updateHotelRoom;
            _deleteHotelRoom = deleteHotelRoom;
        }

        // GET: api/hotelroom
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelRoom>>> GetHotelRooms()
        {
            var hotelRooms = await _getHotelRooms.Execute();
            return Ok(hotelRooms);
        }

        // GET: api/hotelroom/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelRoom>> GetHotelRoom(int id)
        {
            var hotelRoom = await _getHotelRoom.Execute(id);
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
            var createdHotelRoom = await _createHotelRoom.Execute(hotelRoom);
            return CreatedAtAction(nameof(GetHotelRoom), new { id = createdHotelRoom.Id }, createdHotelRoom);
        }

        // PUT: api/hotelroom/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotelRoom(int id, [FromBody] HotelRoom updatedHotelRoom)
        {
            try
            {
                await _updateHotelRoom.Execute(id, updatedHotelRoom);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest("ID mismatch.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/hotelroom/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelRoom(int id)
        {
            try
            {
                await _deleteHotelRoom.Execute(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}