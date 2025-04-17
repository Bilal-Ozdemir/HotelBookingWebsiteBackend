// BackEnd/UseCases/Bookings/GetMyBookings.cs
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

        /// <summary>
        /// Returns only the bookings belonging to the specified userId,
        /// including related HotelRoom, RoomTypes, and Payments.
        /// </summary>
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
