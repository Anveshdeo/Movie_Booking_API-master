using System.ComponentModel.DataAnnotations;

namespace Movie_Booking_API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string LoginId { get; set; }
        public string Password { get; set; }    
        public long ContactNo { get; set; }
        public string Role { get; set; }

    }
}
