// BackEnd/UseCases/Bookings/DeleteBooking.cs
using BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BackEnd.UseCases.Bookings
{
    public class DeleteBooking
    {
        private readonly AppDbContext _context;
        public DeleteBooking(AppDbContext context) => _context = context;

        // Now takes an int userId, matching Booking.UserId
        public async Task<bool> Execute(int bookingId, int userId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null) 
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
