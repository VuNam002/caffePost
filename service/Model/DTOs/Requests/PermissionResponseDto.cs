namespace CaffePOS.Model.DTOs.Requests
{
    public class PermissionResponseDto
    {
        public int permission_id { get; set; }
        public string? permission_name { get; set; }
        public string? description { get; set; }
        public string? module { get; set; }
        public DateTime create_at { get; set; }
    }
}
