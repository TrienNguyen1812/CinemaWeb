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
    }
}
