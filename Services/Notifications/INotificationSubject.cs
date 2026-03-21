using CinemaWeb.Models;
using System.Collections.Generic;

namespace CinemaWeb.Services.Notifications
{
    public interface INotificationSubject
    {
        void Attach(INotificationObserver observer);
        void Detach(INotificationObserver observer);
        void Notify(Notification notification);
        void Publish(string message, string level = "success");
        IReadOnlyList<Notification> GetPublishedNotifications();
    }
}
