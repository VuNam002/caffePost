using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Services
{
    public class RoleService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(AppDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<RoleResponseDto>> GetRoleAll()
        {
            try
            {
                var roles = await _context.Role
                    .Select(roles => new RoleResponseDto
                    {
                        role_id = roles.RoleId,
                        role_name = roles.RoleName,
                        description = roles.Description,
                        created_at = roles.CreatedAt,
                        updated_at = roles.UpdatedAt,
                    }).ToListAsync();
                return roles;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi trong quá trình lấy danh sách phân quyền");
                return new List<RoleResponseDto>();
            }
        }
        //Thêm mới Role
        public async Task<RoleResponseDto> CreateRole(Role role)
        {
            try
            {
                if(role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }
                role.CreatedAt = DateTime.Now;
                role.UpdatedAt = DateTime.Now;

                _context.Role.Add(role);
                await _context.SaveChangesAsync();

                return new RoleResponseDto
                {
                    role_id = role.RoleId,
                    role_name = role.RoleName,
                    description = role.Description,
                    created_at = role.CreatedAt,
                    updated_at = role.UpdatedAt,
                };
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo quyền");
                throw;
            }
        }
        //Chi tiết
        public async Task<RoleResponseDto> Detail(int id)
        {
            try
            {
                var roleResponse = await _context.Role
                    .Where(r => r.RoleId == id)
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .Select(r => new RoleResponseDto
                    {
                        role_id = r.RoleId,
                        role_name = r.RoleName,
                        description = r.Description,
                        created_at = r.CreatedAt,
                        updated_at = r.UpdatedAt,
                        permissions = r.RolePermissions.Select(rp => new PerrmissionResponseDto
                        {
                            permission_id = rp.Permission.PermissionId,
                            permission_name = rp.Permission.PermissionName,
                            description = rp.Permission.Description,
                            module = rp.Permission.Module,
                            created_at = rp.Permission.CreatedAt,
                        }).ToList()
                    }).FirstOrDefaultAsync();
                return roleResponse;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay san pham theo {Id}", id);
                throw;
            }
        }
        // Lấy tất cả các Role
        public async Task<List<RoleResponseDto>> GetAllRoles()
        {
            try
            {
                return await _context.Role
                    .Select(r => new RoleResponseDto
                    {
                        role_id = r.RoleId,
                        role_name = r.RoleName,
                        description = r.Description,
                        created_at = r.CreatedAt,
                        updated_at = r.UpdatedAt,
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay toan bo cac quyen");
                throw;
            }
        }
        // Sửa Role
        public async Task<RoleResponseDto?> EditRole(int id, RolePostDto roleDto)
        {
            try
            {
                // Tìm Role theo ID
                var role = await _context.Role.FindAsync(id);
                if (role == null)
                {
                    _logger.LogWarning("Khong tim thay quyen voi ID: {Id}", id);
                    return null;
                }
                role.RoleName = roleDto.role_name;
                role.Description = roleDto.description;
                role.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return new RoleResponseDto
                {
                    role_id = role.RoleId,
                    role_name = role.RoleName,
                    description = role.Description,
                    created_at = role.CreatedAt,
                    updated_at = role.UpdatedAt,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi cap nhat quyen voi ID: {Id}", id);
                throw;
            }
        }
        //Xóa Role
        public async Task<bool> DeleteRole(int id)
        {
            try
            {
                var role = await _context.Role.FindAsync(id);
                if( role == null)
                {
                    _logger.LogWarning("Khong tim thay quyen voi ID: {Id}", id);
                    return false;
                }
                _context.Role.Remove(role);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Da xoa quyen voi ID: {Id}", id);
                return true;
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi xoa quyen voi ID: {Id}", id);
                return false;
            }
        }
    }
}
