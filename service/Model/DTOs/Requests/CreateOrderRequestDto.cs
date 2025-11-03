namespace CaffePOS.Model.DTOs.Requests
{
    public class CreateOrderRequestDto
    {
        public int UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Customer_phone { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemRequestDto> Items { get; set; } = new();
    }
}