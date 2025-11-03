using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Services;
using Microsoft.AspNetCore.Mvc;


namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        public OrderController(OrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderResponseDto>>> GetAllOrder()
        {
            try
            {
                var order = await _orderService.GetAllOrder();
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all orders.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> Detail(int id)
        {
            try
            {
                var order = await _orderService.Detail(id);
                if(order == null)
                {
                    return NotFound($"Không tìm thấy đơn hàng với id {id}");
                }
                return Ok(order);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details for ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderRequestDto requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdOrder = await _orderService.CreateOrder(requestDto);
                _logger.LogInformation("Order created with id: {OrderId}", createdOrder.order_id);
                return CreatedAtAction(
                    nameof(Detail),
                    new { id = createdOrder.order_id },
                    createdOrder
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng.");
                return StatusCode(500, "Lỗi hệ thống khi tạo đơn hàng. Vui lòng thử lại sau.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrder(id);
                if (!result)
                {
                    return NotFound($"Khong tim thay don hang với ID: {id}");
                }
                return NoContent();
            }
            catch (KeyNotFoundException ex) 
            {
                _logger.LogError(ex, $"Co loi khi xoa don hang {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Co loi khi xoa don hang {id}");
                return StatusCode(500, "Lỗi hệ thống khi xoá đơn hàng.");
            }
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditOrder(int id, [FromBody] OrderPostDto dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Dữ liệu đơn hàng không hợp lệ");
                }
                var updatedOrder = await _orderService.EditOrder(id, dto);
                if(updatedOrder == null)
                {
                    return NotFound($"Không tìm thấy đơn hàng với ID: {id}");
                }
                return Ok(updatedOrder);
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Loi khi cap nhat don hang");
                return StatusCode(500, "Da co loi xay ra khi cap nhat don hang");
            }
        }
        //Cap nhat trang thai san pham
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status )
        {
            try
            {
                var order = await _orderService.UpdateOrderStatus(id, status);
                if(order == null)
                {
                    return NotFound($"Khong tim thay don hang voi ID: {id}");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Loi khi cap nhat trang thai don hang");
                return StatusCode(500, "Da co loi xay ra khi cap nhat trang thai don hang");
            }
        }
    }
}
