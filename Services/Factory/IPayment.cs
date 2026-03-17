namespace CinemaWeb.Services.Factory
{
    public interface IPayment
    {
        void Pay(decimal amount);
    }
    public class CashPayment : IPayment
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine("Thanh toán tiền mặt: " + amount);
        }
    }
    public class MomoPayPayment : IPayment
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine("Thanh toán MomoPay: " + amount);
        }
    }
    public class PaymentFactory
    {
        public static IPayment CreatePayment(string type)
        {
            switch (type)
            {
                case "cash":
                    return new CashPayment();

                case "momopay":
                    return new MomoPayPayment();

                default:
                    throw new Exception("Invalid payment");
            }
        }
    }
}
