using CinemaWeb.Models;

namespace CinemaWeb.Services
{
    public class MovieRepository : IMovieRepository
    {
        private readonly DbContexts _context;

        public MovieRepository(DbContexts context)
        {
            _context = context;
        }

        public IEnumerable<Movie> GetNowShowingMovies()
        {
            return _context.Movies
                           .Where(m => m.ReleaseDate <= DateTime.Now)
                           .ToList();
        }

        public Movie? GetById(int id)
        {
            return _context.Movies
                           .FirstOrDefault(m => m.IdMovie == id);
        }
        public IEnumerable<Movie> GetNowShowingMoviesPaged(int page, int pageSize)
        {
            return _context.Movies
                .Where(m => m.ReleaseDate <= DateTime.Now)
                .OrderByDescending(m => m.ReleaseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int CountNowShowingMovies()
        {
            return _context.Movies
                .Count(m => m.ReleaseDate <= DateTime.Now);
        }
    }
}
