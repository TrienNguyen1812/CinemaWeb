using CinemaWeb.Models;
using CinemaWeb.Services.Decorators;
using System.Linq;

namespace CinemaWeb.Services.Decorators
{
	public class BaseOrderPrice : IPriceComponent
	{
		private readonly Order _order;

		public BaseOrderPrice(Order order)
		{
			_order = order;
		}

        public decimal GetPrice()
        {
            decimal total = 0;

            // Kiểm tra và log nếu cần để debug dễ hơn
            if (_order.Tickets != null && _order.Tickets.Any())
            {
                total += _order.Tickets.Sum(t => t.FinalPrice);
            }

            if (_order.OrderCombos != null && _order.OrderCombos.Any())
            {
                // Đảm bảo Combo không null để tránh cộng với 0 sai logic
                total += _order.OrderCombos.Sum(c => (c.Combo?.Price ?? 0) * c.Quantity);
            }

            return total;
        }
    }
}