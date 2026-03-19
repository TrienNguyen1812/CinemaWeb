using CinemaWeb.Models;
using System.Collections.Generic;

namespace CinemaWeb.Services.Notifications
{
    public class NotificationSubject : INotificationSubject
    {
        private readonly List<INotificationObserver> _observers = new();
        private readonly List<Notification> _notifications = new();

        public void Attach(INotificationObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(INotificationObserver observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        public void Notify(Notification notification)
        {
            _notifications.Add(notification);
            foreach (var observer in _observers)
            {
                observer.Update(notification);
            }
        }

        public void Publish(string message, string level = "success")
        {
            var notification = new Notification
            {
                Message = message,
                Level = level,
                CreatedAt = DateTime.Now
            };
            Notify(notification);
        }

        public IReadOnlyList<Notification> GetPublishedNotifications() => _notifications.AsReadOnly();
    }
}
