using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Movie_Booking_API.Models.DTO
{
    public class GetUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public long ContactNumber { get; set; }
        public string Role { get; set; }

    }
}
