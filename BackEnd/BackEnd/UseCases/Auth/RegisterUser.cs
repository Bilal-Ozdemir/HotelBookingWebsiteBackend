using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using global::BackEnd.BackEnd.Data;

namespace BackEnd.UseCases.Auth
{

    public class RegisterUser
    {
        private readonly AppDbContext _context;

        public RegisterUser(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Execute(RegisterDto registerDto)
        {
            // Check if the email is already registered
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new ArgumentException("Email already exists.");
            }

            // Hash the password before storing
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = hashedPassword
            };

            // Save user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully!";
        }
    }
}
