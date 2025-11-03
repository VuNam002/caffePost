namespace CaffePOS.Model.DTOs.Requests
{
    public class ItemsPostDto
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public decimal? price { get; set; }
        public int category_id { get; set; }
        public string? image_url { get; set; }
        public bool is_active { get; set; }
    }
}
