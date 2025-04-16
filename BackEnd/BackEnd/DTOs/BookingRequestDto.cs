namespace BackEnd.DTOs
{
    public class BookingRequestDto
    {
        public int RoomTypeId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
