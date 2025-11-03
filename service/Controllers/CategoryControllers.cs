using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Services;
using Microsoft.AspNetCore.Mvc;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(CategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<CategoryService.PaginationCategoryResponse>> GetCategoryWithPagination(
            [FromQuery] string? keyword,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "name",
            [FromQuery] bool sortDesc = false)
        {
            try
            {
                var request = new CategoryService.CategorySearchRequest
                {
                    Keyword = keyword,
                    Page = page,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortOrder = sortDesc
                };

                var result = await _categoryService.GetCategoryWithPagination(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm phân trang.");
                return StatusCode(500, "Đã có lỗi khi lấy danh sách sản phẩm.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> Detail(int id)
        {
            try
            {
                var category = await _categoryService.Detail(id);
                if(category == null)
                {
                    return NotFound($"Không tìm thấy danh mục với id {id}");
                }
                return Ok(category);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Không tìm thấy chi tiết danh mục sản phẩm");
                return StatusCode(500, "Đã có lỗi khi lấy chi tiết danh mục sản phẩm");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemResponseDto>>> GetAllCategory()
        {
            try
            {
                var category = await _categoryService.GetAllCategory();
                return Ok(category);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy toàn bộ danh mục sản phẩm");
                return StatusCode(500, "Đã có lỗi xảy ra khi lấy toàn bộ danh sách sản phẩm.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryPostDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Dữ liệu danh mục không hợp lệ");
                }

                // Validation chi tiết hơn
                if (string.IsNullOrWhiteSpace(dto.category_name)) 
                {
                    return BadRequest("Tên danh mục là bắt buộc");
                }

                if (dto.category_name.Length > 100)
                {
                    return BadRequest("Tên danh mục không được vượt quá 100 ký tự");
                }
                var createdCategory = await _categoryService.CreateCategory(dto);
                return Ok(createdCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm danh mục");
                return StatusCode(500, "Đã có lỗi xảy ra khi thêm danh mục");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id);
                if(!result)
                {
                    return NotFound("Không tìm thấy danh mục");
                }
                return Ok("Xóa sảm phẩm thành công");
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Có lỗi khi xóa danh mục có {id}");
                return StatusCode(500, "Đã có lỗi khi xóa danh mục");
            }
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditCategory(int id, [FromBody] CategoryPostDto dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                var updatedCategory = await _categoryService.EditCategory(id, dto);
                if(updatedCategory == null)
                {
                    return NotFound($"Không tìm thấy sản phẩm có id = {id}");
                }
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật sản phẩm có {id}");
                return StatusCode(500, "Đã có lỗi xảy ra khi cập nhật sản phẩm");
            }
        }
    }
}