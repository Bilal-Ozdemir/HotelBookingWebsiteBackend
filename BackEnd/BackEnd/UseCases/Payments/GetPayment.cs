using BackEnd.Entities;
using global::BackEnd.BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Payments
{

    public class GetPayment
    {
        private readonly AppDbContext _context;

        public GetPayment(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> Execute(int id)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
