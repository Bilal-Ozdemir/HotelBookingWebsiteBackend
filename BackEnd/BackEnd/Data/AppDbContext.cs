using Microsoft.EntityFrameworkCore;
using BackEnd.Entities;

namespace BackEnd.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.HotelRoom)
                .WithMany(hr => hr.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HotelRoom>()
                .HasOne(hr => hr.RoomTypes)
                .WithMany(rt => rt.HotelRooms)
                .HasForeignKey(hr => hr.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<HotelRoom>()
                .Property(hr => hr.Price)
                .HasColumnType("decimal(18,2)");

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
        }
    }
}
