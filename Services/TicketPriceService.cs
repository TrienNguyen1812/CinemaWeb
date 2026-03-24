using CinemaWeb.Services.Strategies;

namespace CinemaWeb.Services
{
	public class TicketPriceService
	{
		public decimal CalculatePrice(decimal originalPrice, IPricingStrategy strategy)
		{
			return strategy.CalculatePrice(originalPrice);
		}
	}
}