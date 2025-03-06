
namespace BackEnd.BackEnd.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; } // Foreign key to User
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } // e.g., Credit Card, PayPal

        // Nav property
        public Booking Booking { get; set; }
        public User User { get; set; }
    }
}
