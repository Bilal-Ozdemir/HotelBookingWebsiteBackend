using BackEnd.Data;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Bookings
{

    public class GetBooking
    {
        private readonly AppDbContext _context;

        public GetBooking(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> Execute(int id)
        {
            return await _context.Bookings
               .Include(b => b.HotelRoom)
               .Include(b => b.User)
               .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
