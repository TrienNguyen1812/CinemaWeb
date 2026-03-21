using CinemaWeb.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;

namespace CinemaWeb.Services.Notifications
{
    public class SessionNotificationObserver : INotificationObserver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "Notifications";

        public SessionNotificationObserver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Update(Notification notification)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var raw = context.Session.GetString(SessionKey);
            var notifications = string.IsNullOrEmpty(raw)
                ? new List<Notification>()
                : JsonSerializer.Deserialize<List<Notification>>(raw) ?? new List<Notification>();

            notifications.Add(notification);
            context.Session.SetString(SessionKey, JsonSerializer.Serialize(notifications));
        }
    }
}
