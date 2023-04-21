using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Movie_Booking_API.Models.DTO
{
    public class GetMovie
    {
        
        
        public string MovieName { get; set; }
        public string TheaterName { get; set; }
        public int TicketAllotted { get; set; }
        public int TicketsAvail { get; set; }
        public string Status { get; set; }

        
    }
}
