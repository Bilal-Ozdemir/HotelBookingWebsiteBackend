using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.DTOs;            
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
                    Username = u.Username, 
                    Email    = u.Email
                })
                .ToListAsync();
        }
    }
}
