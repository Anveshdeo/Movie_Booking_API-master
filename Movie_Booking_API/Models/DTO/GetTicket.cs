using System.ComponentModel.DataAnnotations;

namespace Movie_Booking_API.Models.DTO
{
    public class GetTicket
    {
        
        public int TicketId { get; set; }
        public string SeatNo { get; set; }
        public int NoOfTickets { get; set; }
        public string MovieName { get; set; }
        public string TheaterName { get; set; }
    }
}
