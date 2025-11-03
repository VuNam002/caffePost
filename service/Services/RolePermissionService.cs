using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Services
{
    public class RolePermissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RolePermissionService> _logger;

        public RolePermissionService(AppDbContext context, ILogger<RolePermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RolePermissionResponseDto?> GetRolePermissions(int roleId)
        {
            try
            {
                var role = await _context.Role
                    .Where(r => r.RoleId == roleId)
                    .FirstOrDefaultAsync();

                if (role == null)
                {
                    return null;
                }

                var permissionIds = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();

                var permissions = await _context.Permissions
                    .Where(p => permissionIds.Contains(p.PermissionId))
                    .Select(p => new PermissionDetailDto
                    {
                        permission_id = p.PermissionId,
                        permission_name = p.PermissionName,
                        module = p.Module
                    })
                    .ToListAsync();

                var result = new RolePermissionResponseDto
                {
                    role_id = role.RoleId,
                    role_name = role.RoleName,
                    permissions = permissions
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Loi khi lay role va permissions theo role id: {roleId}");
                throw;
            }
        }

        //POST/api/roles/{roleId}/permissions
        public async Task<bool> AssignPermissionsToRole(int roleId, List<int> permissionIds)
        {
            try
            {
                var roleExists = await _context.Role.AnyAsync(r => r.RoleId == roleId);
                if (!roleExists)
                {
                    _logger.LogWarning($"Role id {roleId} không tồn tại");
                    return false;
                }

                var existingRolePermissions = _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId);
                _context.RolePermissions.RemoveRange(existingRolePermissions);

                var newRolePermissions = permissionIds.Select(pid => new Model.RolePermissions
                {
                    RoleId = roleId,
                    PermissionId = pid
                }).ToList();

                _context.RolePermissions.AddRange(newRolePermissions); // Bỏ cast đi
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Đã gán {permissionIds.Count} permissions cho role {roleId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gán permissions cho role id: {roleId}");
                throw;
            }
        }
        // DELETE /api/roles/{roleId}/permissions/{permissionId}
        public async Task<bool> DeletePermission(int roleId, int permissionId)
        {
            try
            {
                //lay id role
                var roleExists = await _context.Role.AnyAsync(r => r.RoleId == roleId);
                if (!roleExists)
                {
                    _logger.LogWarning($"Role id {roleId} không tồn tại");
                    return false;
                }
                //lay id permission
                var permissionExists = await _context.Permissions.AnyAsync(p => p.PermissionId == permissionId);
                if (!permissionExists)
                {
                    _logger.LogWarning($"Permission id {permissionId} không tồn tại");
                    return false;
                }
                //kiem tra permission da duoc gan cho role chua
                var rolePermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
                //dieu kien khong ton tai
                if (rolePermission == null)
                {
                    _logger.LogWarning($"Permission {permissionId} không được gán cho role {roleId}");
                    return false;
                }

                _context.RolePermissions.Remove(rolePermission);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Đã gỡ permission {permissionId} khỏi role {roleId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gỡ permission {permissionId} khỏi role {roleId}");
                throw;
            }
        }
        //PATCH/api/roles/{roleId}/permissions
        public async Task<RolePermissionResponseDto?> EditRolePermission(int roleId, List<int> newPermissionIds)
        {
            try
            {
                var role = await _context.Role.AnyAsync(r => r.RoleId == roleId);
                if (!role)
                {
                    _logger.LogWarning($"Role id {roleId} không tồn tại");
                    return null; // Bây giờ hợp lệ
                }

                //lấy danh sách quyền hiện tại
                var currentPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();

                //Tìm quyền cần thêm và xóa
                var permissionsToAdd = newPermissionIds.Except(currentPermissions).ToList();
                var permissionsToRemove = currentPermissions.Except(newPermissionIds).ToList();

                //Xóa quyền không còn trong danh sách mới
                if (permissionsToRemove.Any())
                {
                    var rolePermissionsToRemove = await _context.RolePermissions
                        .Where(rp => rp.RoleId == roleId && permissionsToRemove.Contains(rp.PermissionId))
                        .ToListAsync();
                    _context.RolePermissions.RemoveRange(rolePermissionsToRemove);
                    _logger.LogInformation($"Đã xóa {rolePermissionsToRemove.Count} permissions khỏi role {roleId}");
                }

                // Tạo biến ở đây để kiểm tra bên ngoài
                var rolePermissionsToAdd = new List<Model.RolePermissions>();

                //Thêm quyền mới
                if (permissionsToAdd.Any())
                {
                    var validPermissionIds = await _context.Permissions
                        .Where(p => permissionsToAdd.Contains(p.PermissionId))
                        .Select(p => p.PermissionId)
                        .ToListAsync();

                    //chỉ thêm quyên hợp lệ
                    rolePermissionsToAdd = validPermissionIds.Select(pid => new Model.RolePermissions
                    {
                        RoleId = roleId,
                        PermissionId = pid
                    }).ToList();

                    _context.RolePermissions.AddRange(rolePermissionsToAdd);
                    _logger.LogInformation($"Đã thêm {rolePermissionsToAdd.Count} permissions cho role {roleId}");

                    // Ghi log nếu có ID không hợp lệ được gửi lên
                    var invalidIds = permissionsToAdd.Except(validPermissionIds).ToList();
                    if (invalidIds.Any())
                    {
                        _logger.LogWarning($"Các permission ids không hợp lệ: {string.Join(", ", invalidIds)}");
                    }
                }

                // 2. Di chuyển SaveChanges và Return ra ngoài
                // Chỉ SaveChanges khi thực sự có thay đổi (thêm hoặc xóa)
                if (permissionsToRemove.Any() || rolePermissionsToAdd.Any())
                {
                    await _context.SaveChangesAsync();
                }

                // Luôn luôn return trạng thái mới nhất (dù có thay đổi hay không)
                return await GetRolePermissions(roleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật permissions cho role id: {roleId}");
                throw;
            }
        }
        //GET/api/permissions/:permissionId/roles
        public async Task<List<RolePermissionResponseDto>> GetRolesByPermission(int permissionId)
        {
            try
            {
                var roles = await _context.RolePermissions
                    .Where(rp => rp.PermissionId == permissionId)
                    .Select(rp => rp.Role)
                    .ToListAsync();
                var result = new List<RolePermissionResponseDto>();
                foreach (var role in roles)
                {
                    var permissions = await _context.RolePermissions
                        .Where(rp => rp.RoleId == role.RoleId)
                        .Select(rp => rp.Permission)
                        .ToListAsync();
                    var permissionDetails = permissions.Select(p => new PermissionDetailDto
                    {
                        permission_id = p.PermissionId,
                        permission_name = p.PermissionName,
                        module = p.Module
                    }).ToList();
                    result.Add(new RolePermissionResponseDto
                    {
                        role_id = role.RoleId,
                        role_name = role.RoleName,
                        permissions = permissionDetails
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy roles theo permission id: {permissionId}");
                throw;
            }
        }
    }
}