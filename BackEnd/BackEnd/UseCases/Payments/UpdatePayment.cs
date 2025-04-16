using BackEnd.Entities;
using global::BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Payments
{

    public class UpdatePayment
    {
        private readonly AppDbContext _context;

        public UpdatePayment(AppDbContext context)
        {
            _context = context;
        }

        public async Task Execute(int id, Payment updatedPayment)
        {
            if (id != updatedPayment.Id)
            {
                throw new ArgumentException("ID mismatch.");
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }

            // Check if the associated booking exists
            var booking = await _context.Bookings.FindAsync(updatedPayment.BookingId);
            if (booking == null)
            {
                throw new ArgumentException("Associated booking not found.");
            }

            // Update payment properties
            payment.Amount = updatedPayment.Amount;
            payment.PaymentMethod = updatedPayment.PaymentMethod;
            payment.PaymentDate = updatedPayment.PaymentDate;

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PaymentExists(id))
                {
                    throw new KeyNotFoundException("Payment not found.");
                }
                throw;
            }
        }

        private async Task<bool> PaymentExists(int id)
        {
            return await _context.Payments.AnyAsync(e => e.Id == id);
        }
    }
}
