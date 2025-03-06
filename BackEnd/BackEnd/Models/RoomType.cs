namespace BackEnd.BackEnd.Models
{
    public class RoomType
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., Single, Double, Suite
        public string Description { get; set; }

        // Nav property
        public ICollection<HotelRoom> HotelRooms { get; set; } 
    }
}
