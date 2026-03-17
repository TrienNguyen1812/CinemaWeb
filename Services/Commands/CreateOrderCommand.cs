namespace CinemaWeb.Services.Commands
{
    public class CreateOrderCommand
    {
        public int IdUser { get; set; }

        public int IdShowtime { get; set; }

        public List<int> SeatIds { get; set; }

        public Dictionary<int, int> Combos { get; set; }
    }
}
