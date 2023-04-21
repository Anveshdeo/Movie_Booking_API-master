using Microsoft.EntityFrameworkCore;

namespace Movie_Booking_API.Models
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().HasKey(m => new { m.MovieName, m.TheaterName });
            modelBuilder.Entity<Movie>().HasData(
                new Movie { MovieName = "TIGER", TheaterName = "PVR", TicketAllotted = 20, TicketsAvail=20, Status = "BOOK ASAP" },
                new Movie { MovieName = "MI", TheaterName = "INOX", TicketAllotted = 10, TicketsAvail=10, Status = "BOOK ASAP" }
                );

            modelBuilder.Entity<User>().HasIndex(m => m.LoginId).IsUnique();
            modelBuilder.Entity<User>().HasIndex(m => m.Email).IsUnique();
            modelBuilder.Entity<User>().HasData(
                new User { Id=1, Email="anvesh@gmail.com", FirstName="Anvesh", LastName="Deo", ContactNo=912334567890, LoginId="deo", Password="123456", Role="Admin"}
                );
        }
    }
}
