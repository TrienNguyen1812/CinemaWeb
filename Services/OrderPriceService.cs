using CinemaWeb.Models;
using CinemaWeb.Services.Decorators;
using System;

namespace CinemaWeb.Services
{
    public class OrderPriceService
    {
        public decimal CalculateTotal(Order order)
        {
            IPriceComponent price = new BaseOrderPrice(order);
            decimal rawTotal = price.GetPrice();

            // 1. Kiểm tra điều kiện tối thiểu để bắt đầu giảm giá (200,000đ)
            if (rawTotal >= 200000)
            {
                // Mốc cơ bản: >= 200k giảm 10%
                decimal discountPercent = 0.10m;

                // 2. Tính số lần tăng thêm mỗi 100,000đ sau mốc 200k
                // Ví dụ: 350k -> (350 - 200) / 100 = 1.5 -> lấy phần nguyên là 1 lần tăng
                decimal extraAmount = rawTotal - 200000;
                int extraSteps = (int)(extraAmount / 200000);

                // Mỗi bước tăng thêm 5%
                discountPercent += (extraSteps * 0.05m);

                // 3. Giới hạn (Cap) mức giảm giá tối đa là 25% (0.25)
                if (discountPercent > 0.25m)
                {
                    discountPercent = 0.25m;
                }

                // 4. Áp dụng Decorator với tổng phần trăm đã tính toán
                price = new PercentageDiscount(price, discountPercent);
            }

            // Trả về kết quả cuối cùng (có thể làm tròn để khớp Database)
            return Math.Round(price.GetPrice(), 2);
        }
    }
}