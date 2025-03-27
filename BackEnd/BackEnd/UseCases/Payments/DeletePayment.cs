using global::BackEnd.BackEnd.Data;

namespace BackEnd.UseCases.Payments
{

    public class DeletePayment
    {
        private readonly AppDbContext _context;

        public DeletePayment(AppDbContext context)
        {
            _context = context;
        }

        public async Task Execute(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
