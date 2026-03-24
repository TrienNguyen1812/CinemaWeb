using CinemaWeb.Models;
using CinemaWeb.Services.Decorators;

namespace CinemaWeb.Services
{
	public class OrderPriceService
	{
		public decimal CalculateTotal(Order order)
		{
			IPriceComponent price = new BaseOrderPrice(order);

			// Chỉ giảm 10% nếu tổng tiền trên 500k chẳng hạn
			if (price.GetPrice() > 500000) 
			{
				price = new PercentageDiscount(price, 0.1m);
			}

			return price.GetPrice();
		}
	}
}