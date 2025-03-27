using BackEnd.Entities;
using global::BackEnd.BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.HotelRooms
{

    public class GetHotelRoom
    {
        private readonly AppDbContext _context;

        public GetHotelRoom(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HotelRoom> Execute(int id)
        {
            return await _context.HotelRooms.Include(hr => hr.RoomTypes)
                .FirstOrDefaultAsync(hr => hr.Id == id);
        }
    }
}
