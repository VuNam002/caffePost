using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CaffePOS.Services;
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy toàn bộ người dùng");
                return StatusCode(500, "Đã có lỗi xảy ra khi lấy toàn bộ tài khoản người dùng");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous] // ⚠️ THÊM: Cho phép login không cần token
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                var token = await _userService.LoginAsync(loginRequest);
                if (token == null)
                {
                    return Unauthorized(new { message = "Ten dang nhap tai khoan mat khau khong dung hoac bi khoa." });
                }
                return Ok(new { token }); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi dang nhap");
                return StatusCode(500, "Da co loi xay ra khi dang nhap");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> Detail(int id)
        {
            try
            {
                var user = await _userService.Detail(id);
                if (user == null)
                {
                    return NotFound($"Khong tim thay nguoi dung voi ID: {id}");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Loi khi lay nguoi dung voi ID: {id}");
                return StatusCode(500, "Da co loi xay ra khi lay thong tin nguoi dung");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserPostDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest("Du lieu nguoi dung khong hop le");
                }
                var user = new Users
                {
                    UserName = userDto.userName,
                    FullName = userDto.fullName,
                    RoleId = userDto.role_id,
                    Email = userDto.email,
                    PhoneNumber = userDto.phoneNumber,
                    IsActive = userDto.is_active,
                    Password = userDto.passWord,
                    Role = new Role(),
                    Orders = new List<Order>()
                };
                var createUser = await _userService.CreateUser(user);
                return Ok(createUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi tao nguoi dung moi");
                return StatusCode(500, "Da co loi xay ra khi tao nguoi dung");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] UserPostDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest("Du lieu nguoi dung khong hop le");
                }
                var result = await _userService.EditUser(id, userDto);
                if (result == null)
                {
                    return NotFound($"Khong tim thay nguoi dung voi ID: {id}");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Loi khi sua nguoi dung voi ID: {id}");
                return StatusCode(500, "Da co loi xay ra khi sua thong tin nguoi dung");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUser(id);
                if (!result)
                {
                    return NotFound($"Khong tim thay nguoi dung voi ID: {id}");
                }
                return Ok(new { message = "Xoa nguoi dung thanh cong" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Loi khi xoa nguoi dung voi ID: {id}");
                return StatusCode(500, "Da co loi xay ra khi xoa nguoi dung");
            }
        }
    }
}