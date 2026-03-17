using CinemaWeb.Models;

namespace CinemaWeb.Services.Builders
{
    public class SeatLayoutBuilder : ISeatBuilder
    {
        private List<Seat> seats = new List<Seat>();

        public void AddRow(string row, int seatCount, int roomId)
        {
            for (int i = 1; i <= seatCount; i++)
            {
                seats.Add(new Seat
                {
                    SeatRow = row,
                    SeatNumber = i.ToString(),
                    IdRoom = roomId,
                    TypeSeat = "Normal"
                });
            }
        }

        public List<Seat> Build()
        {
            return seats;
        }
    }
}
