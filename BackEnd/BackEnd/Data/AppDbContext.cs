using System;
using Microsoft.EntityFrameworkCore;
using BackEnd.Entities;

namespace BackEnd.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User>     Users      { get; set; }
        public DbSet<Booking>  Bookings   { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<Payment>  Payments   { get; set; }
        public DbSet<RoomType> RoomTypes  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships
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

            modelBuilder.Entity<HotelRoom>()
                .HasOne(hr => hr.RoomTypes)
                .WithMany(rt => rt.HotelRooms)
                .HasForeignKey(hr => hr.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "alice", Email = "alice@example.com", PasswordHash = "AQAAAAEAACcQAAAAEFakeHashAlice==" },
                new User { Id = 2, Username = "bob",   Email = "bob@example.com",   PasswordHash = "AQAAAAEAACcQAAAAEFakeHashBob==" }
            );

            // Seed Room Types
            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { Id = 1, Name = "Single", Description = "One person room" },
                new RoomType { Id = 2, Name = "Double", Description = "Two person room" },
                new RoomType { Id = 3, Name = "Suite",  Description = "Premium suite" }
            );

            // Seed multiple HotelRooms per type
            modelBuilder.Entity<HotelRoom>().HasData(
                // Singles
                new HotelRoom { Id =  1, RoomNumber = "101", RoomTypeId = 1, Price = 100m },
                new HotelRoom { Id =  2, RoomNumber = "102", RoomTypeId = 1, Price = 100m },
                new HotelRoom { Id =  3, RoomNumber = "103", RoomTypeId = 1, Price = 100m },
                new HotelRoom { Id =  4, RoomNumber = "104", RoomTypeId = 1, Price = 100m },
                new HotelRoom { Id =  5, RoomNumber = "105", RoomTypeId = 1, Price = 100m },
                // Doubles
                new HotelRoom { Id =  6, RoomNumber = "201", RoomTypeId = 2, Price = 150m },
                new HotelRoom { Id =  7, RoomNumber = "202", RoomTypeId = 2, Price = 150m },
                new HotelRoom { Id =  8, RoomNumber = "203", RoomTypeId = 2, Price = 150m },
                new HotelRoom { Id =  9, RoomNumber = "204", RoomTypeId = 2, Price = 150m },
                new HotelRoom { Id = 10, RoomNumber = "205", RoomTypeId = 2, Price = 150m },
                // Suites
                new HotelRoom { Id = 11, RoomNumber = "301", RoomTypeId = 3, Price = 250m },
                new HotelRoom { Id = 12, RoomNumber = "302", RoomTypeId = 3, Price = 250m },
                new HotelRoom { Id = 13, RoomNumber = "303", RoomTypeId = 3, Price = 250m },
                new HotelRoom { Id = 14, RoomNumber = "304", RoomTypeId = 3, Price = 250m },
                new HotelRoom { Id = 15, RoomNumber = "305", RoomTypeId = 3, Price = 250m }
            );

            // (Optional) Seed additional Bookings & Payments as needed for testing
        }
    }
}