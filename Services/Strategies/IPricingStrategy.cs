namespace CinemaWeb.Services.Strategies
{
	public interface IPricingStrategy
	{
		decimal CalculatePrice(decimal originalPrice);
	}
}
