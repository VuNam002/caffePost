using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Services
{
    public class PermissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(AppDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }
        //Lấy toàn bộ quyền, có thể lọc theo module
        public async Task<List<PermissionResponseDto>> GetAllPermission(string? module = null)
        {
            try
            {
                var query = _context.Permissions.AsQueryable();

                // Filter theo module nếu có
                if (!string.IsNullOrWhiteSpace(module))
                {
                    query = query.Where(p => p.Module == module);
                }

                return await query
                    .Select(p => new PermissionResponseDto
                    {
                        permission_id = p.PermissionId,
                        permission_name = p.PermissionName,
                        description = p.Description,
                        module = p.Module,
                        create_at = p.CreatedAt
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay cac quyen.");
                throw;
            }
        }
        //Thêm quyền mới
        public async Task<PermissionResponseDto> CreatePermission(PermissionPostDto createDto)
        {
            try
            {
                var permission = new Permissions
                {
                    PermissionName = createDto.permission_name,
                    Description = createDto.description,
                    Module = createDto.module
                };
                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                return new PermissionResponseDto
                {
                    permission_id = permission.PermissionId,
                    permission_name = permission.PermissionName,
                    description = permission.Description,
                    module = permission.Module,
                    create_at = permission.CreatedAt
                };
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi tao quyen");
                throw;
            }
        }
        //Xóa quyền
        public async Task<bool> DeletePermission(int id)
        {
            try
            {
                var perrmission = await _context.Permissions.FirstAsync(p => p.PermissionId == id);
                if (perrmission == null)
                {
                    _logger.LogWarning("Khong tim thay quyen de xoa",id);
                    return false;
                }
                _context.Permissions.Remove(perrmission);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Da xoa danh muc ID: {id} thanh cong", id);
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Co loi khi xoa san pham");
                throw;
            }
        }
    }
}