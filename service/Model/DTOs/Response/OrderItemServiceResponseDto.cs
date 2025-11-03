namespace CaffePOS.Model.DTOs.Response
{
    public class OrderItemServiceResponseDto
    {
        public int order_id {  get; set; }
        public List<OrderItemDetailDto>? order_items { get; set; } = new List<OrderItemDetailDto>();
    }
    public class OrderItemDetailDto
    {
        public int order_item_id { get; set; }
        public int item_id { get; set; }
        public string? name { get; set; } = string.Empty;
        public decimal price_at_sale { get; set; }
        public int quantity { get; set; }
        public decimal subtotal { get; set; }
        public string? item_notd { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
