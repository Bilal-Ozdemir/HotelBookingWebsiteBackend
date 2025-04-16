using BackEnd.Data;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
                    .ThenInclude(hr => hr.RoomTypes)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
