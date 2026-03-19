using CinemaWeb.Models;

namespace CinemaWeb.Services.Notifications
{
    public interface INotificationObserver
    {
        void Update(Notification notification);
    }
}
