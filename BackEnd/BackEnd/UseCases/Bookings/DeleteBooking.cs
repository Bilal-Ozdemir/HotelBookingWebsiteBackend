using BackEnd.Data;

namespace BackEnd.UseCases.Bookings
{

    public class DeleteBooking
    {
        private readonly AppDbContext _context;

        public DeleteBooking(AppDbContext context)
        {
            _context = context;
        }

        public async Task Execute(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
