using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Entities
{
    public class HotelRoom
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public int RoomTypeId { get; set; }

        public decimal Price { get; set; } 
        public RoomType RoomTypes { get; set; } 

        public ICollection<Booking> Bookings { get; set; } 
    }
}
