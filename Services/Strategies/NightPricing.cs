namespace CinemaWeb.Services.Strategies
{
	public class NightPricing : IPricingStrategy
	{
		public decimal CalculatePrice(decimal originalPrice)
		{
			return originalPrice * 0.8m; // giảm 20%
		}
	}
}
