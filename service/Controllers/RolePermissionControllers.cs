using CaffePOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/role-permissions")]
    [Authorize]
    public class RolePermissionController : ControllerBase
    {
        private readonly ILogger<RolePermissionController> _logger;
        private readonly RolePermissionService _rolePermissionService;

        public RolePermissionController(
            ILogger<RolePermissionController> logger,
            RolePermissionService rolePermissionService
        )
        {
            _logger = logger;
            _rolePermissionService = rolePermissionService;
        }

        // GET: api/role-permissions/{roleId}
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            try
            {
                var result = await _rolePermissionService.GetRolePermissions(roleId);

                if (result == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Role not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Co loi khi lay role va permission cho role id: {roleId}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }
        // Thêm mới quyền cho role
        [HttpPost("api/roles/{roleId}/permissions")]
        public async Task<IActionResult> AssignPermissionsToRole(
            int roleId,
            [FromBody] List<int> permission_ids)
        {
            if (permission_ids == null || !permission_ids.Any())
            {
                return BadRequest("Permission IDs không được để trống");
            }

            var result = await _rolePermissionService.AssignPermissionsToRole(roleId, permission_ids);

            if (!result)
            {
                return NotFound($"Role {roleId} không tồn tại hoặc permission IDs không hợp lệ");
            }

            return StatusCode(201);
        }
        [HttpDelete("api/roles/{roleId}/permissions/{permissionId}")]
        [Authorize(Policy = "admin")] // hoặc Admin
        public async Task<IActionResult> DeletePermission(int roleId, int permissionId)
        {
            try
            {
                var result = await _rolePermissionService.DeletePermission(roleId, permissionId);

                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Role hoặc Permission không tồn tại"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Gỡ quyền thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gỡ permission {permissionId} khỏi role {roleId}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã có lỗi xảy ra"
                });
            }
        }
        [HttpPatch("{roleId}/permissions")]
        public async Task<IActionResult> EditRolePermission(int roleId, [FromBody] List<int> permission_ids)
        {
            if (permission_ids == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Danh sách permission IDs không được để trống"
                });
            }
            try
            {
                var result = await _rolePermissionService.EditRolePermission(roleId, permission_ids);
                if (result == null)
                {
                    // Service trả về null nếu không tìm thấy Role
                    return NotFound(new {
                        success = false, 
                        message = $"Role {roleId} không tồn tại." 
                    });
                }

                // Trả về 200 OK cùng với dữ liệu đã được cập nhật
                return Ok(new {
                    success = true, 
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật permissions cho role id: {roleId}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã có lỗi xảy ra"
                });
            }
        }
        [HttpGet("{permissionId}/role")]
        public async Task<IActionResult> GetRolesByPermission(int permissionId)
        {
            try
            {
                var roles = await _rolePermissionService.GetRolesByPermission(permissionId);
                return Ok(new
                {
                    success = true,
                    data = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy roles cho permission id: {permissionId}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã có lỗi xảy ra"
                });
            }
        }
    }
}