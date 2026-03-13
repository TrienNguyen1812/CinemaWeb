using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Models
{
    public class DbContexts : DbContext
    {
        public DbContexts(DbContextOptions<DbContexts> options)
       : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ScreeningRoom> ScreeningRooms { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== ORDER - USER =====
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.IdUser)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== PAYMENT - ORDER =====
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.IdOrder)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== TICKET - ORDER =====
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Order)
                .WithMany(o => o.Tickets)
                .HasForeignKey(t => t.IdOrder)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== TICKET - SHOWTIME =====
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Showtime)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.IdShowtime)
                .OnDelete(DeleteBehavior.NoAction);

            // ===== TICKET - SEAT =====
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.IdSeat)
                .OnDelete(DeleteBehavior.NoAction);

            // ===== SHOWTIME - MOVIE =====
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Showtimes)
                .HasForeignKey(s => s.IdMovie)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== SHOWTIME - ROOM =====
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.ScreeningRoom)
                .WithMany(r => r.Showtimes)
                .HasForeignKey(s => s.IdRoom)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
