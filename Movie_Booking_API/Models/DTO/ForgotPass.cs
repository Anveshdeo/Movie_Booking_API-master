using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Movie_Booking_API.Models.DTO
{
    public class ForgotPass
    {
        [Required]
        public string LoginId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
