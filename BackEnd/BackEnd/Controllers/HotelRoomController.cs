using BackEnd.Entities;
using BackEnd.UseCases.HotelRooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/hotelroom")]
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
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetHotelRooms()
        {
            try
            {
                var hotelRooms = await _getHotelRooms.Execute(); // This is where it crashes
                return Ok(hotelRooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 ERROR loading hotel rooms:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace); // ✅ full trace
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }


        // ✅ POST: /api/hotelroom
        [HttpPost]
        public async Task<IActionResult> CreateHotelRoom([FromBody] HotelRoom hotelRoom)
        {
            var created = await _createHotelRoom.Execute(hotelRoom);
            return CreatedAtAction(nameof(GetHotelRoom), new { id = created.Id }, created);
        }

        // ✅ PUT: /api/hotelroom/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotelRoom(int id, [FromBody] HotelRoom updatedRoom)
        {
            try
            {
                await _updateHotelRoom.Execute(id, updatedRoom);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest("ID mismatch.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Room not found.");
            }
        }

        // ✅ DELETE: /api/hotelroom/5
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
                return NotFound("Room not found.");
            }
        }
    }
}
