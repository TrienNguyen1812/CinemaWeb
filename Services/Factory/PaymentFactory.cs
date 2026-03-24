namespace CinemaWeb.Services.Factory
{
    public class PaymentFactory
    {
        public static IPayment CreatePayment(string type)
        {
            // Xử lý logic chọn Class và ném lỗi nếu phương thức không tồn tại
            return type?.ToLower() switch
            {
                "cash" => new CashPayment(),
                "momo" => new MomoPayment(),
                "vnpay" => new VnPayPayment(),
                _ => throw new Exception($"Phương thức thanh toán '{type}' không được hỗ trợ.")
            };
        }
    }
}