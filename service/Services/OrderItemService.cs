using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests; // Cần import DTO Request
using CaffePOS.Model.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CaffePOS.Services
{
    public class OrderItemService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderItemService> _logger;

        public OrderItemService(AppDbContext context, ILogger<OrderItemService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OrderItemResponseDto>> GetOrderItems(int orderId)
        {
            try
            {
                var orderExists = await _context.Order.AnyAsync(o => o.OrderId == orderId);
                if (!orderExists)
                {
                    return new List<OrderItemResponseDto>();
                }
                var orderItems = await _context.OrderItem
                    .Where(oi => oi.OrderId == orderId)
                    .Join(
                        _context.Items,
                        orderItem => orderItem.ItemId,
                        item => item.ItemId,
                        (orderItem, item) => new OrderItemResponseDto
                        {
                            order_item_id = orderItem.OrderItemId,
                            order_id = orderItem.OrderId,
                            item_id = orderItem.ItemId,
                            name = item.Name,
                            quantity = orderItem.Quantity,
                            price_at_sale = orderItem.PriceAtSale,
                            subtotal = orderItem.Subtotal,
                            item_notd = orderItem.ItemNotd,
                            created_at = orderItem.CreatedAt,
                            updated_at = orderItem.UpdatedAt
                        }
                    ).ToListAsync();
                return orderItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết đơn hàng cho orderId {OrderId}", orderId);
                throw;
            }
        }

        public async Task<OrderItemResponseDto?> GetOrderItemById(int orderItemId)
        {
            try
            {
                var orderItem = await _context.OrderItem
                    .Where(oi => oi.OrderItemId == orderItemId)
                    .Join(
                        _context.Items,
                        orderItem => orderItem.ItemId,
                        item => item.ItemId,
                        (orderItem, item) => new OrderItemResponseDto
                        {
                            order_item_id = orderItem.OrderItemId,
                            order_id = orderItem.OrderId,
                            item_id = orderItem.ItemId,
                            name = item.Name,
                            quantity = orderItem.Quantity,
                            price_at_sale = orderItem.PriceAtSale,
                            subtotal = orderItem.Subtotal,
                            item_notd = orderItem.ItemNotd,
                            created_at = orderItem.CreatedAt,
                            updated_at = orderItem.UpdatedAt
                        }
                    ).FirstOrDefaultAsync();
                return orderItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết món hàng cho orderItemId {OrderItemId}", orderItemId);
                throw;
            }
        }

        public async Task<OrderItemResponseDto> CreateOrderItem(int orderId, OrderItemCreateDto dto)
        {
            try
            {
                var order = await _context.Order.FindAsync(orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                var item = await _context.Items.FindAsync(dto.ItemId);
                if (item == null)
                {
                    throw new Exception("Item not found");
                }

                // (Tùy chọn) Kiểm tra xem item có active không
                // if (!item.IsActive) 
                // {
                //    throw new Exception("Item is not available");
                // }

                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    ItemId = dto.ItemId,         
                    Quantity = dto.Quantity,   
                    PriceAtSale = item.Price,
                    Subtotal = item.Price * dto.Quantity,
                    ItemNotd = dto.ItemNotd,   
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.OrderItem.AddAsync(orderItem);
                await _context.SaveChangesAsync();

                var resultDto = await GetOrderItemById(orderItem.OrderItemId);
                if (resultDto == null)
                {
                    throw new Exception("Failed to retrieve the created order item.");
                }

                return resultDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm món hàng vào đơn hàng {OrderId}", orderId);
                throw;
            }
        }
        //cap nhat don hang
        public async Task<OrderItemResponseDto?> EditOrderItem(int id, OrderItemEditDto dto)
        {
            try
            {
                var orderItem = await _context.OrderItem.FirstOrDefaultAsync(oi => oi.OrderItemId == id);
                if( orderItem == null)
                {
                    _logger.LogWarning("Không tìm thấy món hàng với ID {OrderItemId} để cập nhật", id);
                    return null;
                }
                orderItem.Quantity = dto.quantity;
                orderItem.ItemNotd = dto.item_notd;

                await _context.SaveChangesAsync();
                return new OrderItemResponseDto
                {
                    order_item_id = orderItem.OrderItemId,
                    order_id = orderItem.OrderId,
                    item_id = orderItem.ItemId,
                    quantity = orderItem.Quantity,
                    price_at_sale = orderItem.PriceAtSale,
                    subtotal = orderItem.Subtotal,
                    item_notd = orderItem.ItemNotd,
                    created_at = orderItem.CreatedAt,
                    updated_at = orderItem.UpdatedAt
                };
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật món hàng trong đơn hàng {OrderItemId}", id);
                throw;
            }
        }
        //Xóa đơn hàng
        public async Task<bool> DeletedOrderItem(int id)
        {
            try
            {
                var orderItem = await _context.OrderItem
                    .FirstOrDefaultAsync(oi => oi.OrderItemId == id);
                if (orderItem == null)
                {
                    _logger.LogWarning("Khong tim thay mon hang can xoa");
                    return false;
                }
                var orderItems = _context.OrderItem.Remove(orderItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Da xoa mon hang ID: {id} thanh cong", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa món hàng trong đơn hàng {OrderItemId}", id);
                throw;
            }
        }
    }
}