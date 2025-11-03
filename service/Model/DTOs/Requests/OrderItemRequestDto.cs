namespace CaffePOS.Model.DTOs.Requests
{
    public class OrderItemRequestDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
