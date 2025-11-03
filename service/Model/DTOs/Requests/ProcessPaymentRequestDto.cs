namespace CaffePOS.Model.DTOs.Requests
{
    public class ProcessPaymentRequestDto
    {
        public int order_id { get; set; }
        public decimal amount { get; set; }
        public string method { get; set; } = "Tiên mặt";
    }
}
