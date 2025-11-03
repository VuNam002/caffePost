namespace CaffePOS.Model.DTOs.Requests
{
    public class OrderItemCreateDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string? ItemNotd { get; set; }
    }
}
