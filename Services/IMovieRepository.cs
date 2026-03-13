using CinemaWeb.Models;

namespace CinemaWeb.Services
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetNowShowingMovies();
        IEnumerable<Movie> GetNowShowingMoviesPaged(int page, int pageSize);
        int CountNowShowingMovies();
        Movie? GetById(int id);
    }
}
