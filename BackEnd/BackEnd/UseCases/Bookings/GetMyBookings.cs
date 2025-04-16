// BackEnd/UseCases/Bookings/GetMyBookings.cs
using BackEnd.Data;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.UseCases.Bookings
{
    public class GetMyBookings
    {
        private readonly AppDbContext _context;
        public GetMyBookings(AppDbContext context) => _context = context;

        // Now takes an int userId, matching Booking.UserId
        public async Task<IEnumerable<Booking>> Execute(int userId)
        {
            return await _context.Bookings
               .Include(b => b.HotelRoom)
               .Where(b => b.UserId == userId)
               .ToListAsync();
        }
    }
}
