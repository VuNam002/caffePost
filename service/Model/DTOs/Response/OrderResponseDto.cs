namespace CaffePOS.Model.DTOs.Response
{
    public class OrderResponseDto
    {
        // Nếu muốn trả về danh sách sản phẩm trong đơn hàng, nên dùng property public và kiểu phù hợp
        public List<OrderItemResponseDto>? items { get; set; }

        public int order_id { get; set; }
        public DateTime? order_date { get; set; }
        public decimal total_amount { get; set; }
        public decimal discount_amount { get; set; }
        public decimal final_amount { get; set; }
        public string? status { get; set; }
        public string? notes { get; set; }
        public int user_id { get; set; }
        public string? customer_name { get; set; }
        public string? customer_phone { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
