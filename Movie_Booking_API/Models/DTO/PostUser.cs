using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Movie_Booking_API.Models.DTO
{
    public class PostUser
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
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



        [Required]
        //[RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Please Enter Valid Contact Number")]
        public long Contact { get; set; }

    }
}
