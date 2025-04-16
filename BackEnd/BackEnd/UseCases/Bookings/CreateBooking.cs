using BackEnd.Data;
using BackEnd.Entities;
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

        public async Task<Booking> Execute(int userId, int roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            var availableRoom = await _context.HotelRooms
                .Where(r => r.RoomTypeId == roomTypeId)
                .Where(r => !_context.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    (
                        (checkIn >= b.CheckIn && checkIn < b.CheckOut) ||
                        (checkOut > b.CheckIn && checkOut <= b.CheckOut) ||
                        (checkIn <= b.CheckIn && checkOut >= b.CheckOut)
                    )))
                .FirstOrDefaultAsync();

            if (availableRoom == null)
                throw new InvalidOperationException("No available room for the selected dates.");

            var booking = new Booking
            {
                UserId = userId,
                RoomId = availableRoom.Id,
                CheckIn = checkIn,
                CheckOut = checkOut,
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }
    }
}
