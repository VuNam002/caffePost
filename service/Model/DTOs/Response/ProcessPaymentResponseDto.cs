namespace CaffePOS.Model.DTOs.Response
{
    public class ProcessPaymentResponseDto
    {
        /// ID của giao dịch thanh toán vừa được tạo.
        public int payment_id { get; set; }

        /// ID của đơn hàng vừa được thanh toán.
        public int order_id { get; set; }

        /// Tổng số tiền phải trả của đơn hàng (lấy từ Order.final_amount).
        public decimal total_amount_due { get; set; }

        /// Số tiền thực tế khách hàng đã đưa (lấy từ Payment.amount).
        public decimal amount_paid { get; set; }
        /// Đây là kết quả tính toán: (amount_paid - total_amount_due)
        public decimal change { get; set; }
        /// Thông báo trạng thái, ví dụ: "Thanh toán thành công!"
        public string message { get; set; }
    }
}