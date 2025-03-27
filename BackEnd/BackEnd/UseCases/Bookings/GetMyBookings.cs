using BackEnd.Entities;
using BackEnd.BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackEnd.UseCases.Bookings
{

    public class GetMyBookings
    {
        private readonly AppDbContext _context;

        public GetMyBookings(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> Execute(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not logged in.");
            }

            return await _context.Bookings
               .Where(b => b.UserId == int.Parse(userId))
               .ToListAsync();
        }
    }
}
