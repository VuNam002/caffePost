using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    [Authorize] // Thêm Authorization Bearer token
    public class PermissionController : ControllerBase
    {
        private readonly PermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(PermissionService permissionService, ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<PermissionResponseDto>>> GetAllPermission(
            [FromQuery] string? module = null) 
        {
            try
            {
                var permissions = await _permissionService.GetAllPermission(module);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all permissions.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionPostDto dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Permission data is null.");
                }
                var permission = new Permissions
                {
                    PermissionName = dto.permission_name,
                    Description = dto.description,
                    Module = dto.module
                };
                await _permissionService.CreatePermission(dto);
                return Ok(permission);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new permission.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var result = await _permissionService.DeletePermission(id);
                if(!result)
                {
                    return NotFound("Khong tim thay quyen");
                }
                return Ok("Xoa quyen thanh cong");
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Co loi khi xoa quyen");
                return StatusCode(500, "Da co loi khi xoa quyen");
            }
        }
    }
}