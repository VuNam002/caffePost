namespace CaffePOS.Model.DTOs.Requests
{
    public class CategoryPostDto
    {
        public string? category_name { get; set; }
        public string? description { get; set; }
        public bool is_active { get; set; }
    }
}
