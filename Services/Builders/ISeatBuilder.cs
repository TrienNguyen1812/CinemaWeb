using CinemaWeb.Models;

namespace CinemaWeb.Services.Builders
{
    public interface ISeatBuilder
    {
        void AddRow(string row, int seatCount, int roomId);
        List<Seat> Build();
    }
}
