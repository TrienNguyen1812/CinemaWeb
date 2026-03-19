using CinemaWeb.Models;
using CinemaWeb.Services;
using CinemaWeb.Services.Commands;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddNewtonsoftJson();

builder.Services.AddDbContext<DbContexts>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<CreateOrderCommandHandler>();

// Observer pattern services for notification
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CinemaWeb.Services.Notifications.INotificationSubject, CinemaWeb.Services.Notifications.NotificationSubject>();
builder.Services.AddScoped<CinemaWeb.Services.Notifications.INotificationObserver, CinemaWeb.Services.Notifications.SessionNotificationObserver>();
builder.Services.AddScoped<CinemaWeb.Services.Notifications.INotificationObserver, CinemaWeb.Services.Notifications.ConsoleNotificationObserver>();

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
