namespace CaffePOS.Model.DTOs.Response
{
    public class UserResponseCreateDto
    {
        public string? userName { get; set; }
        public string? passWord { get; set; }
        public string? fullName { get; set; }
        public int role_id { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public bool? is_active { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
