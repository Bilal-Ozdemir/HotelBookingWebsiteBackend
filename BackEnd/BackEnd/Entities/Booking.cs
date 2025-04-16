namespace BackEnd.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime BookingDate { get; set; }

        public User User { get; set; }
        public HotelRoom HotelRoom { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
