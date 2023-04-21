using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie_Booking_API.Models
{
    public class Movie
    {
        [Key]
        [Column(Order = 0)]
        public string MovieName { get; set; }

        [Key]
        [Column(Order = 1)]
        public string TheaterName { get; set; }
        public int TicketAllotted { get; set; }

        public int TicketsAvail { get; set; }
        public string Status { get; set; }



        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
