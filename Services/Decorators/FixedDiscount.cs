namespace CinemaWeb.Services.Decorators
{
	public class FixedDiscount : PriceDecorator
	{
		private readonly decimal _amount;

		public FixedDiscount(IPriceComponent component, decimal amount)
			: base(component)
		{
			_amount = amount;
		}

		public override decimal GetPrice()
		{
			return _component.GetPrice() - _amount;
		}
	}
}