// BackEnd/UseCases/Bookings/DeleteBooking.cs
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Bookings
{
    public class DeleteBooking
    {
        private readonly AppDbContext _context;

        public DeleteBooking(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Deletes the booking and any linked payments, if it belongs to the given userId.
        /// </summary>
        public async Task<bool> Execute(int bookingId, int userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null)
                return false;

            
            if (booking.Payments != null && booking.Payments.Any())
            {
                _context.Payments.RemoveRange(booking.Payments);
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
