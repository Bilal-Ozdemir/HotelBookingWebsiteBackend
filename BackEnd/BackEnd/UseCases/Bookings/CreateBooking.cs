using BackEnd.Entities;
using BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.UseCases.Bookings
{

    public class CreateBooking
    {
        private readonly AppDbContext _context;

        public CreateBooking(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> Execute(Booking booking)
        {
            var room = await _context.HotelRooms.FindAsync(booking.RoomId);
            if (room == null) throw new ArgumentException("Room not found.");

            // Check for room availability
            bool isRoomBooked = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                (
                    booking.CheckIn >= b.CheckIn && booking.CheckIn < b.CheckOut ||
                    booking.CheckOut > b.CheckIn && booking.CheckOut <= b.CheckOut ||
                    booking.CheckIn <= b.CheckIn && booking.CheckOut >= b.CheckOut
                )
            );

            if (isRoomBooked) throw new InvalidOperationException("Room is already booked for the selected dates.");

            booking.UserId = booking.UserId; // Assuming this is set externally
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }
    }
}
