using BackEnd.Entities;
using global::BackEnd.BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Payments
{
 
    public class GetPayments
    {
        private readonly AppDbContext _context;

        public GetPayments(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> Execute()
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.User)
                .ToListAsync();
        }
    }
}
