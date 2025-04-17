// BackEnd/UseCases/Admin/DeleteUser.cs
using System.Threading.Tasks;
using BackEnd.Data;

namespace BackEnd.UseCases.Admin
{
    public class DeleteUser
    {
        private readonly AppDbContext _context;
        public DeleteUser(AppDbContext context) => _context = context;

        public async Task<bool> Execute(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
