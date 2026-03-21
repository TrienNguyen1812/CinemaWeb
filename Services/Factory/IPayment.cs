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

    public class MomoPayment : IPayment
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine("Thanh toán MoMo: " + amount);
        }
    }

    public class VnPayPayment : IPayment
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine("Thanh toán VNPay: " + amount);
        }
    }

    public class PaymentFactory
    {
        public static IPayment CreatePayment(string type)
        {
            switch (type?.ToLower())
            {
                case "cash":
                    return new CashPayment();
                case "momo":
                case "momopay":
                    return new MomoPayment();
                case "vnpay":
                    return new VnPayPayment();
                default:
                    throw new Exception("Invalid payment method");
            }
        }
    }
}
