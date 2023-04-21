using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_API.Models;
using Movie_Booking_API.Models.DTO;
using System.Data;
using System.Net.Sockets;

namespace Movie_Booking_API.Controllers
{
    
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MovieBookingController : ControllerBase
    {
        private MovieDbContext _db;
        public MovieBookingController(MovieDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getTickets")]
        public IActionResult GetAllTicket()
        {
            var tickets = _db.Tickets.ToList();
            var resultsDTO = new List<GetTicket>();
            tickets.ToList().ForEach(ticket =>
            {
                var resultDTO = new GetTicket()
                {
                    TicketId = ticket.TicketId,
                    NoOfTickets = ticket.NoOfTickets,
                    SeatNo = ticket.SeatNo,
                    MovieName = ticket.MovieName,
                    TheaterName = ticket.TheaterName,



                };
                resultsDTO.Add(resultDTO);
            });

            return Ok(resultsDTO);
        }


        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public IActionResult GetTicket(int id)
        {
            var ticket = _db.Tickets.Find(id);

            if (ticket == null)
            {
                return NotFound("Ticket Not Found");
            }
            var tickets = new Models.DTO.GetTicket()
            {
                TicketId = ticket.TicketId,
                NoOfTickets = ticket.NoOfTickets,
                SeatNo = ticket.SeatNo,
                MovieName = ticket.MovieName,
                TheaterName = ticket.TheaterName,
            };

            return Ok(tickets);
        }


        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public IActionResult GetMovies()
        {
            var movie = _db.Movies.ToList();
            var resultsDTO = new List<Models.DTO.GetMovie>();
            movie.ToList().ForEach(result =>
            {
                var resultDTO = new Models.DTO.GetMovie()
                {
                    MovieName = result.MovieName,
                    TheaterName = result.TheaterName,
                    TicketAllotted = result.TicketAllotted,
                    TicketsAvail = result.TicketsAvail,
                    Status = result.Status
                };
                resultsDTO.Add(resultDTO);
            });



            return Ok(resultsDTO);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("[action]")]
        public IActionResult FindMovies(string movieName)
        {
            var movies = from movie in _db.Movies
                         where movie.MovieName.StartsWith(movieName)
                         select
                         new
                         {
                             MovieName = movie.MovieName,
                             TheaterName = movie.TheaterName,
                             TicketAllotted = movie.TicketAllotted,
                             TicketsAvail = movie.TicketsAvail,
                             Status = movie.Status

                         };

            return Ok(movies);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("changeMovieStatus")]
        public IActionResult PutMovieStatus(string movieName, string status)
        {
            var allowedStatus = new List<string>() { "BOOK ASAP", "SOLD OUT" };
            if (movieName == null || !allowedStatus.Contains(status))
            {
                return NotFound("Please enter correct movie Name/status");
            }

            var result = _db.Movies.FirstOrDefault(m => m.MovieName == movieName);

            result.Status = status;
            _db.SaveChanges();
            var resultDTO = new GetMovie()
            {
                MovieName = result.MovieName,
                TheaterName = result.TheaterName,
                TicketAllotted = result.TicketAllotted,
                TicketsAvail= result.TicketsAvail,
                Status = result.Status
            };
            return Ok(resultDTO);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult add([FromBody] AddTicket addticket)
        {
            var movies = _db.Movies.FirstOrDefault(m => m.MovieName == addticket.MovieName);

            if (movies == null)
            {
                return NotFound();
            }

            if(movies.Status == "SOLD OUT")
            {
                return NotFound("Tickets are Sold Out");
            }


            int ticketsAvailable = movies.TicketAllotted - _db.Tickets
            .Where(t => t.MovieName == addticket.MovieName).Sum(t => t.NoOfTickets);



            if (ticketsAvailable < addticket.NoOfTickets)
            {
                return BadRequest($"Only {ticketsAvailable} tickets are available");
            }

            var ticket = new Ticket
            {
                NoOfTickets = addticket.NoOfTickets,
                SeatNo = GenerateSeats(addticket.MovieName, addticket.NoOfTickets),
                MovieName = addticket.MovieName,
                TheaterName = addticket.TheaterName,
                Movie = movies
            };
            
            movies.TicketsAvail -= addticket.NoOfTickets;

            if(movies.TicketsAvail == 0)
            {
                movies.Status = "SOLD OUT";
            }

            _db.Tickets.Add(ticket);
            _db.SaveChanges();

            var resultDTO = new Models.DTO.GetTicket()
            {
                TicketId = ticket.TicketId,
                NoOfTickets = ticket.NoOfTickets,
                SeatNo = ticket.SeatNo,
                MovieName = ticket.MovieName,
                TheaterName = ticket.TheaterName,
            };

            return CreatedAtAction("GetTicket", new { id = resultDTO.TicketId }, resultDTO);
        }



        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, User")]
        public IActionResult DeleteTicket(int id)
        {
            var ticket = _db.Tickets.Find(id);

            if (ticket == null)
            {
                return NotFound("Ticket Not Found");
            }

            var movies = _db.Movies.FirstOrDefault(m => m.MovieName == _db.Tickets.Find(id).MovieName);

            if (movies.TicketsAvail == 0)
            {
                movies.Status = "BOOK ASAP";
            }

            movies.TicketsAvail += ticket.NoOfTickets;


            _db.Tickets.Remove(ticket);
            _db.SaveChanges();

            var tickets = new GetTicket()
            {
                TicketId = ticket.TicketId,
                NoOfTickets = ticket.NoOfTickets,
                SeatNo = ticket.SeatNo,
                MovieName = ticket.MovieName,
                TheaterName = ticket.TheaterName,
            };

            return Ok(tickets);
        }

        private string GenerateSeats(string movieName, int NoOfTickets)
        {
            var ticketsList = _db.Tickets.ToList();
            var bookedSeats = ticketsList.Where(x => x.MovieName == movieName)
            .SelectMany(t => t.SeatNo.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Distinct()
            .ToList();

            var newSeats = new List<string>();
            var row = 'A';
            var col = 1;
            for (int i = 0; i < NoOfTickets; i++)
            {
                string seatNumber = $"{row}{col}";
                while (bookedSeats.Contains(seatNumber))
                {
                    col++;
                    if (col > 10)
                    {
                        row++;
                        col = 1;
                    }
                    seatNumber = $"{row}{col}";

                }
                newSeats.Add(seatNumber);
                bookedSeats.Add(seatNumber);

                //moving to next seat
                col++;
                if (col > 10)
                {
                    row++;
                    col = 1;
                }
            }

            return string.Join(',', newSeats);

        }
    }
}
