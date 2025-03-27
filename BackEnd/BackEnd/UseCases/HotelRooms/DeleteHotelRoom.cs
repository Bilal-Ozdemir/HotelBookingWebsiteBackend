using global::BackEnd.BackEnd.Data;
namespace BackEnd.UseCases.HotelRooms
{

    public class DeleteHotelRoom
    {
        private readonly AppDbContext _context;

        public DeleteHotelRoom(AppDbContext context)
        {
            _context = context;
        }

        public async Task Execute(int id)
        {
            var hotelRoom = await _context.HotelRooms.FindAsync(id);
            if (hotelRoom == null)
            {
                throw new KeyNotFoundException("HotelRoom not found.");
            }

            _context.HotelRooms.Remove(hotelRoom);
            await _context.SaveChangesAsync();
        }
    }
}
