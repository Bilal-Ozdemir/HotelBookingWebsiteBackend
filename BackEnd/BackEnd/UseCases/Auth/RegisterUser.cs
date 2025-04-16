using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Data;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.UseCases.Auth
{
    public class RegisterUser
    {
        private readonly AppDbContext _context;

        public RegisterUser(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Execute(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                throw new Exception("Email already exists");

            var hasher = new PasswordHasher<User>();
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };
            user.PasswordHash = hasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Registration successful";
        }
    }
}