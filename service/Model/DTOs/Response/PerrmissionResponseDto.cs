namespace CaffePOS.Model.DTOs.Response
{
    public class PerrmissionResponseDto
    {
        public int permission_id { get; set; }
        public string? permission_name { get; set; }
        public string? description { get; set; }
        public string? module { get; set; }
        public DateTime created_at { get; set; }
    }
}
