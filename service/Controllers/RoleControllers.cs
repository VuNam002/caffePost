using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Services;
using Microsoft.AspNetCore.Mvc;

// Namespace của Controller
namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(RoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleResponseDto>>> GetAllRole()
        {
            try
            {
                var roles = await _roleService.GetRoleAll();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy danh sách vai trò.");
                return StatusCode(500, "Đã xảy ra lỗi ở máy chủ, vui lòng thử lại sau.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RolePostDto dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Du lieu quyen khong hop le");
                }
                var role = new Role
                {
                    RoleName = dto.role_name,
                    Description = dto.description,
                };
                var createRole = await _roleService.CreateRole(role);
                return Ok(createRole);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm danh muc");
                return StatusCode(500, "Da co loi khi them danh muc");
            }
        }
        [HttpGet("id")]
        public async Task<ActionResult<RoleResponseDto>> Detail(int id)
        {
            try
            {
                var role = await _roleService.Detail(id);
                if(role == null)
                {
                    return NotFound($"Khong tim thay quyen voi id {id}");
                }
                return Ok(role);
            } catch (Exception ex)
            {
                _logger.LogError($"Failed to detail {id}", ex);
                return StatusCode(500, "Da co loi khi lay chi tiet quyen");
            }
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditRole(int id, [FromBody] RolePostDto dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Du lieu quyen khong hop le");
                }
                var editRole = await _roleService.EditRole(id, dto);
                if(editRole == null)
                {
                    return NotFound($"Khong tim thay quyen voi id {id}");
                }
                return Ok(editRole);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi sửa quyền");
                return StatusCode(500, "Đã có lỗi khi sửa quyền");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var result = await _roleService.DeleteRole(id);
                if (!result)
                {
                    return NotFound($"Khong tim thay quyen voi id {id}");
                }
                return Ok("Xoa quyen thanh cong");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa quyền");
                return StatusCode(500, "Đã có lỗi khi xóa quyền");
            }
        }
    }
}