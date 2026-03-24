using CinemaWeb.Services.Decorators;

namespace CinemaWeb.Services.Decorators
{
	public abstract class PriceDecorator : IPriceComponent
	{
		protected IPriceComponent _component;

		public PriceDecorator(IPriceComponent component)
		{
			_component = component;
		}

		public virtual decimal GetPrice()
		{
			return _component.GetPrice();
		}
	}
}