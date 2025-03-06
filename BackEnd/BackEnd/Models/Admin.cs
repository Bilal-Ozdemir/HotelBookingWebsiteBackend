using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.BackEnd.Models
{
    public class Admin : IdentityUser
    {
        public int Id { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Nav property
        public User User { get; set; }

    }
}
