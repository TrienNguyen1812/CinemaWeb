namespace CinemaWeb.Services.Strategies
{
	public class NormalPricing : IPricingStrategy
	{
		public decimal CalculatePrice(decimal originalPrice)
		{
			return originalPrice;
		}
	}
}
