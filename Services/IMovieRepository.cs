using CinemaWeb.Models;

namespace CinemaWeb.Services
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetNowShowingMovies();
        IEnumerable<Movie> GetNowShowingMoviesPaged(int page, int pageSize);
        int CountNowShowingMovies();
        // 👉 THÊM CÁC DÒNG NÀY ĐỂ HẾT LỖI
        IEnumerable<Movie> GetComingSoonMovies();
        IEnumerable<Movie> GetComingSoonMoviesPaged(int page, int pageSize);
        int CountComingSoonMovies();

        IEnumerable<Movie> GetAllMovies();
        Movie? GetById(int id);
        Movie? GetMovieDetail(int id);
    }
}
