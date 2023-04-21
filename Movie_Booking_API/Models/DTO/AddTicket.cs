namespace Movie_Booking_API.Models.DTO
{
    public class AddTicket
    {
        public string MovieName { get; set; }
        public string TheaterName
        {
            get; set;
        }
        public int NoOfTickets { get; set; }

    }
}
