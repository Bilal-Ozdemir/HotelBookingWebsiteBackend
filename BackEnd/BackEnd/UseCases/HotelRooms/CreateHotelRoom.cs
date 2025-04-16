using BackEnd.Entities;
using global::BackEnd.Data;

namespace BackEnd.UseCases.HotelRooms
{

    public class CreateHotelRoom
    {
        private readonly AppDbContext _context;

        public CreateHotelRoom(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HotelRoom> Execute(HotelRoom hotelRoom)
        {
            _context.HotelRooms.Add(hotelRoom);
            await _context.SaveChangesAsync();
            return hotelRoom; // Return the created hotel room
        }
    }
}
