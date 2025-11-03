namespace CaffePOS.Model.DTOs.Response
{
    public class CategoryResponseDto
    {
        public int category_id { get; set; }
        public string? category_name { get; set;}
        public string? description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool is_active { get; set; }
    }
}
