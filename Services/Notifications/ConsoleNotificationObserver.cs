using CinemaWeb.Models;
using System;

namespace CinemaWeb.Services.Notifications
{
    public class ConsoleNotificationObserver : INotificationObserver
    {
        public void Update(Notification notification)
        {
            Console.WriteLine($"[Notification] {notification.CreatedAt:O} [{notification.Level}] {notification.Message}");
        }
    }
}
