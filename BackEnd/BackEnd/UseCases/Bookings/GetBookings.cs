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
               // Include the related HotelRoom and its RoomTypes
               .Include(b => b.HotelRoom)
                   .ThenInclude(hr => hr.RoomTypes)
               // Include any Payments made for the booking
               .Include(b => b.Payments)
               // Optionally include User if needed elsewhere
               .Include(b => b.User)
               .ToListAsync();
        }
    }
}