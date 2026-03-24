namespace CinemaWeb.Services.Decorators
{
	public class PercentageDiscount : PriceDecorator
	{
		private readonly decimal _percent;

		public PercentageDiscount(IPriceComponent component, decimal percent)
			: base(component)
		{
			_percent = percent;
		}

		public override decimal GetPrice()
		{
			return _component.GetPrice() * (1 - _percent);
		}
	}
}