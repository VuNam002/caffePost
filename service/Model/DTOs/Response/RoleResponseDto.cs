namespace CaffePOS.Model.DTOs.Response
{
    public class RoleResponseDto
    {
        public int role_id { get; set; }
        public string? role_name { get; set; }
        public string? description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public List<PerrmissionResponseDto>? permissions { get; set; }
    }
}
