using System;

namespace CinemaWeb.Models
{
    public class Notification
    {
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Level { get; set; } = "info"; // info, success, warning, danger
    }
}
