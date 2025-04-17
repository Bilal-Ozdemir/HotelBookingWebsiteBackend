// BackEnd/UseCases/Admin/GetAllUsers.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.DTOs;             // ← Import the DTO namespace
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Admin
{
    public class GetAllUsers
    {
        private readonly AppDbContext _context;
        public GetAllUsers(AppDbContext context) => _context = context;

        public async Task<IEnumerable<UserDto>> Execute()
        {
            return await _context.Users
                .Select(u => new UserDto {
                    Id       = u.Id,
                    Username = u.Username,  // ← Use your actual entity property
                    Email    = u.Email
                })
                .ToListAsync();
        }
    }
}
