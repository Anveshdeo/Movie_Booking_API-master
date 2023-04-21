using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie_Booking_API.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        public int NoOfTickets { get; set; }
        public string SeatNo { get; set; }
        public string MovieName { get; set; }
        public string TheaterName { get; set; }

        [ForeignKey("MovieName, TheaterName")]
        public virtual Movie Movie { get; set; }

    }
}
