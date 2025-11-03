using BCrypt.Net;
using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace CaffePOS.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext context, ILogger<UserService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<UserResponseDto>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(user => new UserResponseDto
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        RoleId = user.RoleId,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt,
                    })
                    .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi trong quá trình lấy danh sách người dùng");
                return new List<UserResponseDto>();
            }
        }
        public async Task<string?> LoginAsync(LoginRequestDto loginRequest)
        {
            // Tìm người dùng theo tên đăng nhập
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName);

            // Kiểm tra nếu người dùng không tồn tại hoặc tài khoản bị khóa
            if (user is null || user.IsActive != true)
            {
                return null; // Trả về null để báo hiệu đăng nhập thất bại
            }

            // Xác thực mật khẩu
            // So sánh mật khẩu người dùng nhập (loginRequest.Password) với mật khẩu đã băm trong CSDL (user.Password)
            bool isPasswordValid = global::BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);

            if (!isPasswordValid)
            {
                return null; // Sai mật khẩu
            }

            // Nếu mật khẩu đúng, tạo và trả về JWT token
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Subject chứa các thông tin (claims) về người dùng sẽ được mã hóa vào token
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString()) // Ví dụ: thêm vai trò (role) vào token
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Thời gian token hết hạn
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<UserResponseDto?> Detail(int id)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.UserId == id)
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        UserName = u.UserName,
                        FullName = u.FullName,
                        RoleId = u.RoleId,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                    })
                    .FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy người dùng theo ID: {Id}", id);
                throw;
            }
        }
        public async Task<UserResponseDto> CreateUser(Users userDto)
        {
            try
            {
                var newUser = new Users
                {
                    UserName = userDto.UserName,
                    Password = global::BCrypt.Net.BCrypt.HashPassword(userDto.Password), // Băm mật khẩu trước khi lưu
                    FullName = userDto.FullName,
                    RoleId = userDto.RoleId,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Role = await _context.Role.FindAsync(userDto.RoleId) ?? throw new InvalidOperationException("Role not found"),
                    Orders = new List<Order>()
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return new UserResponseDto
                {
                    UserId = newUser.UserId,
                    UserName = newUser.UserName,
                    FullName = newUser.FullName,
                    RoleId = newUser.RoleId,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    IsActive = newUser.IsActive,
                    CreatedAt = newUser.CreatedAt,
                    UpdatedAt = newUser.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo người dùng mới");
                throw;
            }
        }
        public async Task<UserResponseDto?> EditUser(int id, UserPostDto userDto)
        {
            try
            {
                var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

                if (userEntity == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với ID: {Id}", id);
                    return null; 
                }
                userEntity.FullName = userDto.fullName;
                userEntity.RoleId = userDto.role_id;
                userEntity.Email = userDto.email;
                userEntity.PhoneNumber = userDto.phoneNumber;
                userEntity.IsActive = userDto.is_active;
                userEntity.UpdatedAt = DateTime.UtcNow;


                if (!string.IsNullOrWhiteSpace(userDto.passWord))
                {
                    userEntity.Password = BCrypt.Net.BCrypt.HashPassword(userDto.passWord);
                }

                await _context.SaveChangesAsync();
                var response = new UserResponseDto
                {
                    UserId = userEntity.UserId,
                    UserName = userEntity.UserName,
                    FullName = userEntity.FullName,
                    RoleId = userEntity.RoleId,
                    Email = userEntity.Email,
                    PhoneNumber = userEntity.PhoneNumber,
                    IsActive = userEntity.IsActive,
                    CreatedAt = userEntity.CreatedAt,
                    UpdatedAt = userEntity.UpdatedAt
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật người dùng với ID: {Id}", id);
                throw; 
            }
        }
        public async Task<bool>DeleteUser (int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if( user == null)
                {
                    _logger.LogWarning("Khong tim thay nguoi dung");
                    return false;
                }
                _context.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Xoa nguoi dung thanh cong");
                return true;
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Co loi xay ra khi xoa nguoi dung");
                return false;
            }
        }
    }
}