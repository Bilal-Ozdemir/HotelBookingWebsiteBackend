using Microsoft.EntityFrameworkCore;
using BackEnd.BackEnd.Models; 

namespace BackEnd.BackEnd.Data  
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
