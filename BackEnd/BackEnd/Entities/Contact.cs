using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Entities
{
    public class Contact
    {
        public int Id { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

         [RegularExpression(@"^(\+?\d{1,3}[- ]?)?\(?\d{1,4}?\)?[- ]?\d{1,4}[- ]?\d{1,9}$", 
        ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        public User
    }
}