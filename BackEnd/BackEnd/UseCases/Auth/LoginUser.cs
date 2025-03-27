using BackEnd.BackEnd.Data;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Auth
{
    public class LoginUser
    {
        private readonly AppDbContext _context;

        public LoginUser(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> Execute(LoginDto loginDto)
        {
            // Find the user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // Validate user existence and password match
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return user; // Return the user object
        }
    }
}
