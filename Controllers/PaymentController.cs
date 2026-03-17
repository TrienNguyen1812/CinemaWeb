using CinemaWeb.Services.Commands;
using CinemaWeb.Services.Factory;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly CreateOrderCommandHandler _handler;

        public PaymentController(CreateOrderCommandHandler handler)
        {
            _handler = handler;
        }
        public IActionResult Checkout(int orderId, string paymentType, decimal total)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var payment = PaymentFactory.CreatePayment(paymentType);

            payment.Pay(total);

            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
            return View();
        }
    }
}
