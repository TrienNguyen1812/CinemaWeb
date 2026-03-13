using CinemaWeb.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaWeb.Services
{
    public class AuthService
    {
        private static AuthService _instance;
        private readonly IServiceScopeFactory _scopeFactory;

        private AuthService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // Singleton Instance
        public static AuthService GetInstance(IServiceScopeFactory scopeFactory)
        {
            if (_instance == null)
            {
                _instance = new AuthService(scopeFactory);
            }
            return _instance;
        }

        // ===== REGISTER =====
        public void Register(User user)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DbContexts>();

            context.Users.Add(user);
            context.SaveChanges();
        }

        // ===== CHECK EMAIL =====
        public bool EmailExists(string email)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DbContexts>();

            return context.Users.Any(u => u.Email == email);
        }

        // ===== LOGIN =====
        public User Login(string email, string password)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DbContexts>();

            return context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
    }
}