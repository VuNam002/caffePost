namespace CaffePOS.Model.DTOs.Requests
{
    public class UserPostDto
    {
        public string? userName { get; set; }
        public string? fullName { get; set; }
        public string? passWord { get; set; }
        public int role_id { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public bool is_active { get; set; }
    }
}
