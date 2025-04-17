using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.DTOs;             
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Admin
{
    public class UpdateUser
    {
        private readonly AppDbContext _context;
        public UpdateUser(AppDbContext context) => _context = context;

        public async Task<bool> Execute(int userId, string newUsername, string newEmail)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            user.Username = newUsername;
            user.Email    = newEmail;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
