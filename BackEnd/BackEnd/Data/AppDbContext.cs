using Microsoft.EntityFrameworkCore;
using BackEnd.BackEnd.Models; 

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

        }
    }
}
