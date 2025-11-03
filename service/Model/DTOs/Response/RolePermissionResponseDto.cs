namespace CaffePOS.Model.DTOs.Response
{
    public class RolePermissionResponseDto
    {
        public int role_id { get; set; }
        public string? role_name { get; set; } = string.Empty;
        public List<PermissionDetailDto>? permissions { get; set; } = new List<PermissionDetailDto>();
    }
    public class PermissionDetailDto
    {
        public int permission_id { get; set; }
        public string? permission_name { get; set; } = string.Empty;
        public string? module { get; set; } = string.Empty;
    }
}
