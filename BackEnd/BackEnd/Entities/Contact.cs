using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Entities
{
    public class Contact
    {
        public int Id { get; set; }

        [ForeignKey("User")] 
        public int UserId { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public User User { get; set; } 
    }
}