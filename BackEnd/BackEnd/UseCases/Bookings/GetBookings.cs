using BackEnd.Entities;
using BackEnd.Data;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BackEnd.UseCases.Bookings
{


    public class GetBookings
    {
        private readonly AppDbContext _context;

        public GetBookings(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> Execute()
        {
            return await _context.Bookings
               .Include(b => b.HotelRoom)
               .Include(b => b.User)
               .ToListAsync();
        }
    }
}
