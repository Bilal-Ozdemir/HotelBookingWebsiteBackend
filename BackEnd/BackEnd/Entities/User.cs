using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
// File: BackEnd/Entities/User.cs
namespace BackEnd.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        // New field for role (e.g., "Admin" or "User")
        [Required]
        public string Role { get; set; } = "User";

        // Navigation properties...
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
