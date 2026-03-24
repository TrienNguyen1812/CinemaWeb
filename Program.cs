using CinemaWeb.Models;
using CinemaWeb.Services;
using CinemaWeb.Services.Commands;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
builder.Services.AddDbContext<DbContexts>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<CreateOrderCommandHandler>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<TicketPriceService>();
builder.Services.AddScoped<OrderPriceService>();

// Observer pattern services for notification
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CinemaWeb.Services.Notifications.INotificationSubject, CinemaWeb.Services.Notifications.NotificationSubject>();
builder.Services.AddScoped<CinemaWeb.Services.Notifications.INotificationObserver, CinemaWeb.Services.Notifications.SessionNotificationObserver>();
builder.Services.AddScoped<CinemaWeb.Services.Notifications.INotificationObserver, CinemaWeb.Services.Notifications.ConsoleNotificationObserver>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

// Đăng ký Background Service
builder.Services.AddHostedService<MovieStatusWorker>();

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
