namespace CinemaWeb.Services.Strategies
{
    public class WednesdayPricing : IPricingStrategy
    {
        public decimal CalculatePrice(decimal basePrice)
        {
            // Mặc định mọi vé là 45,000đ theo yêu cầu
            return 45000m;
        }
    }
}