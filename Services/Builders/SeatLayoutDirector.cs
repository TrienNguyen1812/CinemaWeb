namespace CinemaWeb.Services.Builders
{
    public class SeatLayoutDirector
    {
        public void BuildStandardRoom(ISeatBuilder builder, int roomId)
        {
            builder.AddRow("A", 10, roomId);
            builder.AddRow("B", 10, roomId);
            builder.AddRow("C", 10, roomId);
            builder.AddRow("D", 10, roomId);
            builder.AddRow("E", 10, roomId);
            builder.AddRow("F", 10, roomId);
        }
    }
}
