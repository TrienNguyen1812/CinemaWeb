using CinemaWeb.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class MovieStatusWorker : BackgroundService
{
    private readonly IServiceProvider _services;

    public MovieStatusWorker(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DbContexts>();
                var today = DateTime.Today;

                // Tìm các phim "Sắp chiếu" nhưng đã đến hoặc qua ngày phát hành
                var moviesToUpdate = context.Movies
                    .Where(m => m.Status == "Sắp chiếu" && m.ReleaseDate <= today)
                    .ToList();

                if (moviesToUpdate.Any())
                {
                    foreach (var movie in moviesToUpdate)
                    {
                        movie.Status = "Đang chiếu";
                    }
                    await context.SaveChangesAsync();
                }
            }

            // Chạy kiểm tra mỗi 24 giờ
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}