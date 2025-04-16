using BackEnd.Entities;
using global::BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.HotelRooms
{

    public class GetHotelRooms
    {
        private readonly AppDbContext _context;

        public GetHotelRooms(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<HotelRoom>> Execute()
        {
            return await _context.HotelRooms.Include(hr => hr.RoomTypes).ToListAsync();
        }
    }
}
