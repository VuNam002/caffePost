namespace CaffePOS.Model.DTOs.Requests
{
    public class OrderPostDto
    {
        public DateTime order_date { get; set; }
        public decimal total_amount { get; set; }
        public decimal? discount_amount { get; set; }
        public decimal final_amount { get; set; }
        public string? status { get; set; }
        public string? notes { get; set; }
        public int user_id { get; set; }
        public string? customer_name { get; set; }
    }
}
