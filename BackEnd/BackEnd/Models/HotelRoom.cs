namespace BackEnd.BackEnd.Models
{
    public class HotelRoom
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public int RoomTypeId { get; set; } // Foreign Key for RoomType
        public decimal Price { get; set; }

        // Nav property
        public ICollection<Booking> Bookings { get; set; }
        public RoomType RoomTypes { get; set; }
    }
}
