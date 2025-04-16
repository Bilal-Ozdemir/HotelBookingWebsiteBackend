using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Data;
using Microsoft.AspNetCore.Identity;
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

        public async Task<User> Execute(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) throw new Exception("Invalid credentials");

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid credentials");

            return user;
        }
    }
}