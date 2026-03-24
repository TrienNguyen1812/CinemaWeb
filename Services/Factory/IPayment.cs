public interface IPayment
{
    void Pay(decimal amount);
    string GetOrderStatus();    // Trạng thái đơn hàng (Paid, Pending...)
    string GetPaymentStatus();  // Trạng thái bản ghi thanh toán
    string GenerateTransactionCode(); // Tạo mã giao dịch
    string GetDisplayName();
    string GetSuccessMessage();
}

public class MomoPayment : IPayment
{
    public void Pay(decimal amount) => Console.WriteLine("Thanh toán MoMo: " + amount);
    public string GetOrderStatus() => PaymentConstants.OrderPaid;
    public string GetPaymentStatus() => PaymentConstants.StatusPaid;
    public string GenerateTransactionCode() => "LOCAL-MOMO-" + DateTime.Now.Ticks;
    public string GetDisplayName() => PaymentConstants.Momo;
    public string GetSuccessMessage() => "Thanh toán qua MoMo thành công!";
}

// Tương tự cho CashPayment và VnPayPayment...
public class CashPayment : IPayment
{
    public void Pay(decimal amount) => Console.WriteLine("Thanh toán tiền mặt: " + amount);
    public string GetOrderStatus() => PaymentConstants.OrderPending;
    public string GetPaymentStatus() => PaymentConstants.StatusPending;
    public string GenerateTransactionCode() => "LOCAL-CASH-" + DateTime.Now.Ticks;
    public string GetDisplayName() => PaymentConstants.Cash;
    public string GetSuccessMessage() => "Đặt vé thành công! Vui lòng thanh toán tại quầy để xác nhận.";
}

public class VnPayPayment : IPayment
{
    public void Pay(decimal amount) => Console.WriteLine("Thanh toán VnPay: " + amount);
    public string GetOrderStatus() => PaymentConstants.OrderPaid;
    public string GetPaymentStatus() => PaymentConstants.StatusPaid;
    public string GenerateTransactionCode() => "LOCAL-VNPAY-" + DateTime.Now.Ticks;
    public string GetDisplayName() => PaymentConstants.VNPay;
    public string GetSuccessMessage() => "Thanh toán qua VNPay thành công!";
}