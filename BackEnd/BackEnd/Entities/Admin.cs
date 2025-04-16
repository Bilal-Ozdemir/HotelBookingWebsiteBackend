using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Entities
{
    public class Admin : IdentityUser
    {
        [EmailAddress]
        [Required]
        public override string Email { get; set; }

        
    }
}
