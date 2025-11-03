using CaffePOS.Model.DTOs.Requests; // Cần import DTO Request
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Services;
using Microsoft.AspNetCore.Mvc;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/orders/{order_id}/items")]
    public class OrderItemController : ControllerBase
    {
        private readonly ILogger<OrderItemController> _logger;
        private readonly OrderItemService _orderItemService;

        public OrderItemController(
            ILogger<OrderItemController> logger,
            OrderItemService orderItemService
        )
        {
            _logger = logger;
            _orderItemService = orderItemService;
        }

        // ... (Hàm GetOrderItems giữ nguyên) ...
        [HttpGet]
        public async Task<IActionResult> GetOrderItems(int order_id)
        {
            try
            {
                var result = await _orderItemService.GetOrderItems(order_id);

                if (result == null || result.Count == 0)
                {
                    return Ok(new
                    {
                        success = true,
                        data = new List<object>()
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
                _logger.LogError(ex, $"Error retrieving order items for order id: {order_id}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }

        // ... (Hàm GetSingleOrderItem giữ nguyên) ...
        [HttpGet("{order_item_id}")]
        public async Task<IActionResult> GetSingleOrderItem(int order_id, int order_item_id)
        {
            try
            {
                var result = await _orderItemService.GetOrderItemById(order_item_id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Order item not found"
                    });
                }
                if (result.order_id != order_id)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Order item does not belong to the specified order"
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
                _logger.LogError(ex, $"Error retrieving single order item: {order_item_id}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }
        //Route: POST /api/orders/{order_id}/items
        [HttpPost]
        public async Task<IActionResult> CreateOrderItem(int order_id, [FromBody] OrderItemCreateDto dto)
        {
            try
            {
                // Truyền dto vào service
                var createdItem = await _orderItemService.CreateOrderItem(order_id, dto);

                return CreatedAtAction(nameof(GetSingleOrderItem), new { order_id = order_id, order_item_id = createdItem.order_item_id }, new
                {
                    success = true,
                    data = createdItem
                });
            }
            catch (Exception ex)
            {
                // Thêm exception message để debug dễ hơn
                _logger.LogError(ex, $"Error creating order item for order id: {order_id}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error: " + ex.Message // Trả về lỗi
                });
            }
        }
        //Router: PATCH /api/orders/{order_id}/items/{order_item_id}
        [HttpPatch("{order_item_id}")]
        public async Task<IActionResult> EditOrderItem(int order_id, int order_item_id, [FromBody] OrderItemEditDto dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Du lieu khong hop le");
                }
                var updatedItem = await _orderItemService.EditOrderItem(order_item_id ,dto);
                if (updatedItem == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Order item not found"
                    });
                }
                if (updatedItem.order_id != order_id)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Order item does not belong to the specified order"
                    });
                }
                return Ok(new
                {
                    success = true,
                    data = updatedItem
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order item: {order_item_id}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }
        //Route: DELETE /api/orders/{order_id}/items/{order_item_id}
        [HttpDelete("{order_item_id}")]
        public async Task<IActionResult> DeleteOrderItem(int order_id, int order_item_id)
        {
            try
            {
                var deleted = await _orderItemService.DeletedOrderItem(order_item_id);
                if (!deleted)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Khong tim thay don hang can xoa"
                    });
                }
                return Ok(new
                {
                    success = true,
                    message = "Da xoa thanh cong"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order item: {order_item_id}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }
    }
}