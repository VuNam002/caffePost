namespace CaffePOS.Model.DTOs.Response
{
    public class PaymentResponseDto
    {
        public int payment_id { get; set; }
        public int order_id { get; set; }
        public DateTime payment_date { get; set; }
        public decimal amount { get; set; }
        public string? method { get; set; }
        public string? transaction_id { get; set; }
        public string? notes { get; set; }
    }
}
