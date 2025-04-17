using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Bookings
{
    public class GetMyBookings
    {
        private readonly AppDbContext _context;

        public GetMyBookings(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Booking>> Execute(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.HotelRoom)
                    .ThenInclude(hr => hr.RoomTypes)
                .Include(b => b.Payments)
                .ToListAsync();
        }
    }
}
