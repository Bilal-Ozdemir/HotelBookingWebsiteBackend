using Microsoft.EntityFrameworkCore;
using BackEnd.BackEnd.Models;
using Microsoft.AspNetCore.Identity;
namespace BackEnd.BackEnd.Data  
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<HotelRoom> HotelRooms {  get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        public async Task EnsureSeedData(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Admin>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                const string adminEmail = "admin@example.com";
                const string adminPassword = "Admin@123";

                // Check if the admin role exists; if not, create it
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Check if the admin user already exists
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new Admin
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true // Confirm the email automatically
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        // Assign admin role
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Admin and User is a 1 to 1 relation
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserId);

            // User and Booking is a 1 to many, where 1 user can have many different Bookings
            // Also Bookings has a Foreign key to User for nav and relation
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking and HotelRoom is a 1 to many, where each booking can have many HotelRooms
            // 1 HotelRoom can have many Bookings
            // Booking has a foreign Key to HotelRoom for nav and relation
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.HotelRoom)
                .WithMany(hr => hr.Bookings)
                .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

            // Payment and Booking is a 1 to many, where each booking can have many payments
            // Payment has a Foreign Key to Booking for nav and relation
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment and User is a 1 to many, where each User can have many payments
            // Payment has a Foreign Key to User for nav and relation
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // HotelRoom and RoomType is a 1 to many, where each RoomType can have many HotelRooms
            // HotelRoom has a Foreign Key to RoomType for nav and relation
            modelBuilder.Entity<HotelRoom>()
                .HasOne(hr => hr.RoomTypes)
                .WithMany(rt => rt.HotelRooms)
                .HasForeignKey(hr => hr.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "john_doe", Email = "john@example.com", PasswordHash = "hashed_password_1" },
                new User { Id = 2, Username = "jane_smith", Email = "jane@example.com", PasswordHash = "hashed_password_2" }
            );

            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { Id = 1, Name = "Single", Description = "A room for one person." },
                new RoomType { Id = 2, Name = "Double", Description = "A room for two people." },
                new RoomType { Id = 3, Name = "Suite", Description = "A spacious room with a separate living area." }
            );

            modelBuilder.Entity<HotelRoom>().HasData(
                new HotelRoom { Id = 1, RoomNumber = "101", RoomTypeId = 1, Price = 100.00m },
                new HotelRoom { Id = 2, RoomNumber = "102", RoomTypeId = 2, Price = 150.00m },
                new HotelRoom { Id = 3, RoomNumber = "201", RoomTypeId = 3, Price = 250.00m }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking { Id = 1, UserId = 1, RoomId = 1, CheckIn = new DateTime(2025, 3, 8), CheckOut = new DateTime(2025, 3, 12) },
                new Booking { Id = 2, UserId = 2, RoomId = 2, CheckIn = new DateTime(2025, 3, 9), CheckOut = new DateTime(2025, 3, 13) }
            );

            modelBuilder.Entity<Payment>().HasData(
                new Payment { Id = 1, BookingId = 1, UserId = 1, Amount = 400.00m, PaymentDate = new DateTime(2025, 3, 8), PaymentMethod = "Credit Card" },
                new Payment { Id = 2, BookingId = 2, UserId = 2, Amount = 600.00m, PaymentDate = new DateTime(2025, 3, 9), PaymentMethod = "PayPal" }
            );
        }
    }
}
