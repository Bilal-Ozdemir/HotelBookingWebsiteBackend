using BackEnd.Entities;
using global::BackEnd.BackEnd.Data;

namespace BackEnd.UseCases.Payments
{

    public class CreatePayment
    {
        private readonly AppDbContext _context;

        public CreatePayment(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> Execute(Payment payment)
        {
            var booking = await _context.Bookings.FindAsync(payment.BookingId);
            if (booking == null)
            {
                throw new ArgumentException("Booking not found.");
            }

            payment.PaymentDate = DateTime.UtcNow; // Set the payment date to now
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment; // Return the created payment
        }
    }
}
