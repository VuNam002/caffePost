using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Services;
using Microsoft.AspNetCore.Mvc;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ItemsService _itemsService;
        private readonly ILogger<ItemsController> _logger;
        public ItemsController(ItemsService itemsService, ILogger<ItemsController> logger)
        {
            _itemsService = itemsService;
            _logger = logger;
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<ItemsService.PaginatedItemsResponse>> GetItemsWithPagination(
            [FromQuery] string? keyword,
            [FromQuery] int? categoryId,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "name",
            [FromQuery] bool sortDesc = false)
        {
            try
            {
                var request = new ItemsService.ItemSearchRequest
                {
                    Keyword = keyword,
                    CategoryId = categoryId,
                    IsActive = isActive,
                    Page = page,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDesc = sortDesc
                };

                var result = await _itemsService.GetItemsWithPagination(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm phân trang");
                return StatusCode(500, "Đã có lỗi xảy ra khi lấy danh sách sản phẩm");
            }
        }

        // Lấy chi tiết sản phẩm
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponseDto>> Detail(int id)
        {
            try
            {
                var item = await _itemsService.Detail(id); 
                if (item == null)
                {
                    return NotFound($"Không tìm thấy sản phẩm với id {id}");
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy chi tiết sản phẩm {id}");
                return StatusCode(500, "Đã có lỗi khi lấy chi tiết sản phẩm");
            }
        }
        // Thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemsPostDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dữ liệu sản phẩm không hợp lệ");
                // Validation cơ bản
                if (string.IsNullOrWhiteSpace(dto.name))
                    return BadRequest("Tên sản phẩm là bắt buộc");
                if (dto.price <= 0)
                    return BadRequest("Giá sản phẩm phải lớn hơn 0");

                var item = new Items
                {
                    Name = dto.name,
                    Description = dto.description,
                    Price = dto.price ?? 0,
                    CategoryId = dto.category_id,
                    ImageUrl = dto.image_url,
                    IsActive = dto.is_active
                };
                var createdItem = await _itemsService.CreateItem(item); 
                return CreatedAtAction(nameof(Detail), new { id = createdItem.item_id }, createdItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm mới");
                return StatusCode(500, "Đã có lỗi xảy ra khi thêm sản phẩm");
            }
        }

        // Sửa sản phẩm
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditItem(int id, [FromBody] ItemsPostDto itemDto)
        {
            try
            {
                if (itemDto == null)
                    return BadRequest("Dữ liệu không hợp lệ");
                var updatedItem = await _itemsService.EditItem(id, itemDto); 
                if (updatedItem == null)
                    return NotFound($"Không tìm thấy sản phẩm có id = {id}");
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật sản phẩm {id}");
                return StatusCode(500, "Đã có lỗi xảy ra khi cập nhật sản phẩm");
            }
        }
        //Cập nhật trạng thái sản phẩm
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateItemStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedItem = await _itemsService.UpdateItemStatus(id, isActive); 
                if (updatedItem == null)
                    return NotFound($"Không tìm thấy sản phẩm có id = {id}");
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật trạng thái sản phẩm {id}");
                return StatusCode(500, "Đã có lỗi xảy ra khi cập nhật trạng thái sản phẩm");
            }
        }

        // Xóa sản phẩm
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var result = await _itemsService.DeleteItem(id); 
                if (!result)
                {
                    return NotFound("Không tìm thấy sản phẩm để xóa");
                }
                return Ok("Xóa sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa sản phẩm {id}");
                return StatusCode(500, "Đã có lỗi xảy ra khi xóa sản phẩm"); 
            }
        }
        // Thêm endpoint lấy tất cả sản phẩm (nếu cần)
        [HttpGet]
        public async Task<ActionResult<List<ItemResponseDto>>> GetAllItems()
        {
            try
            {
                var items = await _itemsService.GetAllItems();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy toàn bộ sản phẩm");
                return StatusCode(500, "Đã có lỗi xảy ra khi lấy danh sách sản phẩm");
            }
        }
    }
}