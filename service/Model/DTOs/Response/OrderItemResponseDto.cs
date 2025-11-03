namespace CaffePOS.Model.DTOs.Response
{

    public class OrderItemResponseDto
    {
        public int order_item_id { get; set; }
        public int item_id { get; set; }
        public int quantity { get; set; }
        public decimal price_at_sale { get; set; }
        public decimal subtotal { get; set; }
        public string? item_notd { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        // Thông tin join từ bảng Items
        public string? name { get; set; }

        // Khóa ngoại trỏ về Order
        public int order_id { get; set; }
    }
}