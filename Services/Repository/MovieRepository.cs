using CinemaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Services
{
    public class MovieRepository : IMovieRepository
    {
        private readonly DbContexts _context;

        public MovieRepository(DbContexts context)
        {
            _context = context;
        }

        public IEnumerable<Movie> GetAllMovies()
        {
            return _context.Movies.ToList();
        }

        public IEnumerable<Movie> GetNowShowingMovies()
        {
            var today = DateTime.Today;
            return _context.Movies
                .Where(m => (m.Status == MovieStatus.Showing) || 
                            (m.Status == MovieStatus.ComingSoon && m.ReleaseDate <= today))
                .ToList();
        }

        public IEnumerable<Movie> GetComingSoonMovies()
        {
            var today = DateTime.Today;
            return _context.Movies
                .Where(m => m.Status == MovieStatus.ComingSoon && m.ReleaseDate > today)
                .ToList();
        }

        public Movie? GetById(int id)
        {
            return _context.Movies
                           .FirstOrDefault(m => m.IdMovie == id);
        }
        public IEnumerable<Movie> GetNowShowingMoviesPaged(int page, int pageSize)
        {
            var today = DateTime.Today;
            return _context.Movies
                .Where(m => (m.Status == MovieStatus.Showing) || 
                            (m.Status == MovieStatus.ComingSoon && m.ReleaseDate <= today)) // Thêm điều kiện này cho giống hàm GetNowShowingMovies
                .OrderByDescending(m => m.ReleaseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public IEnumerable<Movie> GetComingSoonMoviesPaged(int page, int pageSize)
        {
            var today = DateTime.Today;
            return _context.Movies
                .Where(m => m.Status == MovieStatus.ComingSoon && m.ReleaseDate > today)
                .OrderByDescending(m => m.ReleaseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int CountNowShowingMovies()
        {
            return _context.Movies
                .Count(m => m.Status == MovieStatus.Showing);
        }

        public int CountComingSoonMovies()
        {
            return _context.Movies
                .Count(m => m.Status == MovieStatus.ComingSoon);
        }

        public Movie? GetMovieDetail(int id)
        {
            return _context.Movies
                .Include(m => m.Showtimes)
                .ThenInclude(s => s.ScreeningRoom)
                .ThenInclude(r => r.Cinema)
                .FirstOrDefault(m => m.IdMovie == id);
        }

        public List<TopMovieDto> GetTopMovies(int topN)
        {
            int currentYear = DateTime.Now.Year;

            var topMovies = _context.Tickets
                .Include(t => t.Order) // Cần Include Order để check Status
                .Include(t => t.Showtime).ThenInclude(s => s.Movie)
                .Where(t => t.Showtime.Movie != null && 
                            (t.Order.Status == PaymentConstants.OrderPaid) && // Chỉ tính vé đã trả tiền
                            t.Order.OrderTime.Year == currentYear) // Chỉ tính trong năm hiện tại
                .GroupBy(t => new { t.Showtime.Movie.IdMovie, t.Showtime.Movie.MovieName })
                .Select(g => new TopMovieDto
                {
                    MovieName = g.Key.MovieName,
                    TotalTickets = g.Count(),
                    // Sử dụng FinalPrice (giá sau cùng) sẽ chính xác hơn Price gốc
                    TotalRevenue = g.Sum(t => (decimal?)t.FinalPrice) ?? 0 
                })
                .OrderByDescending(m => m.TotalTickets)
                .Take(topN)
                .ToList();

            // Gán thứ hạng
            for (int i = 0; i < topMovies.Count; i++)
            {
                topMovies[i].Rank = i + 1;
            }

            return topMovies;
        }
    }
}
