using System.ComponentModel.DataAnnotations;

namespace CaffePOS.Model.DTOs.Requests
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string? Password { get; set; }
    }
}
