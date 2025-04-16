using BackEnd.Entities;
using global::BackEnd.Data;
using Microsoft.EntityFrameworkCore;
namespace BackEnd.UseCases.HotelRooms
{

    public class UpdateHotelRoom
    {
        private readonly AppDbContext _context;

        public UpdateHotelRoom(AppDbContext context)
        {
            _context = context;
        }

        public async Task Execute(int id, HotelRoom updatedHotelRoom)
        {
            if (id != updatedHotelRoom.Id)
            {
                throw new ArgumentException("ID mismatch.");
            }

            _context.Entry(updatedHotelRoom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelRoomExists(id))
                {
                    throw new KeyNotFoundException("HotelRoom not found.");
                }
                throw;
            }
        }

        private async Task<bool> HotelRoomExists(int id)
        {
            return await _context.HotelRooms.AnyAsync(e => e.Id == id);
        }
    }
}
