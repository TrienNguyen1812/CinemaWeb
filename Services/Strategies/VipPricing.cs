namespace CinemaWeb.Services.Strategies
{
	public class VipPricing : IPricingStrategy
	{
		public decimal CalculatePrice(decimal originalPrice)
		{
			return originalPrice * 1.3m; // tăng 30%
		}
	}
}
