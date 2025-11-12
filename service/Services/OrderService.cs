using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OrderResponseDto>> GetAllOrder()
        {
            try
            {
                return await _context.Order
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Item)
                    .Select(o => new OrderResponseDto
                    {
                        order_id = o.OrderId,
                        order_date = o.OrderDate,
                        total_amount = o.TotalAmount,
                        discount_amount = o.DiscountAmount ?? 0,
                        final_amount = o.FinalAmount,
                        status = o.Status.ToString(),
                        notes = o.Notes,
                        user_id = o.UserId,
                        customer_name = o.CustomerName,
                        customer_phone = o.Customer_phone,
                        created_at = o.CreatedAt,
                        updated_at = o.UpdatedAt,
                        items = o.OrderItems.Select(oi => new OrderItemResponseDto
                        {
                            order_item_id = oi.OrderItemId,
                            item_id = oi.ItemId,
                            quantity = oi.Quantity,
                            price_at_sale = oi.PriceAtSale,
                            subtotal = oi.Subtotal,
                            item_notd = oi.ItemNotd,
                            name = oi.Item != null ? oi.Item.Name : null,
                            order_id = oi.OrderId,
                            created_at = oi.CreatedAt,
                            updated_at = oi.UpdatedAt
                        }).ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đơn hàng");
                throw;
            }
        }

        //  Chi tiết đơn hàng
        public async Task<OrderResponseDto?> Detail(int id)
        {
            try
            {
                var order = await _context.Order
                    .Where(o => o.OrderId == id)
                    .Select(o => new OrderResponseDto
                    {
                        order_id = o.OrderId,
                        order_date = o.OrderDate,
                        total_amount = o.TotalAmount,
                        discount_amount = o.DiscountAmount ?? 0,
                        final_amount = o.FinalAmount,
                        status = o.Status.ToString(),
                        notes = o.Notes,
                        user_id = o.UserId,
                        customer_name = o.CustomerName,
                        customer_phone = o.Customer_phone,
                        created_at = o.CreatedAt,
                        updated_at = o.UpdatedAt,
                        items = _context.OrderItem
                            .Where(oi => oi.OrderId == o.OrderId)
                            .Select(oi => new OrderItemResponseDto
                            {
                                order_item_id = oi.OrderItemId,
                                item_id = oi.ItemId,
                                quantity = oi.Quantity,
                                price_at_sale = oi.PriceAtSale,
                                subtotal = oi.Subtotal,
                                item_notd = oi.ItemNotd
                            }).ToList()
                    })
                    .FirstOrDefaultAsync();

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết đơn hàng {Id}", id);
                throw;
            }
        }

        //  Tạo đơn hàng
        public async Task<OrderResponseDto> CreateOrder(CreateOrderRequestDto requestDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (requestDto?.Items == null || !requestDto.Items.Any())
                    throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm.");

                var itemIds = requestDto.Items.Select(i => i.ItemId).Distinct().ToList();
                var itemsFromDb = await _context.Items
                    .Where(p => itemIds.Contains(p.ItemId))
                    .ToDictionaryAsync(p => p.ItemId);

                if (itemsFromDb.Count != itemIds.Count)
                {
                    var missingIds = itemIds.Except(itemsFromDb.Keys);
                    throw new KeyNotFoundException($"Không tìm thấy sản phẩm ID: {string.Join(", ", missingIds)}");
                }

                decimal totalAmount = 0;
                var orderItemsEntities = new List<OrderItem>();

                foreach (var itemDto in requestDto.Items)
                {
                    var item = itemsFromDb[itemDto.ItemId];
                    var subtotal = item.Price * itemDto.Quantity;
                    totalAmount += subtotal;

                    orderItemsEntities.Add(new OrderItem
                    {
                        ItemId = item.ItemId,
                        Quantity = itemDto.Quantity,
                        PriceAtSale = item.Price,
                        Subtotal = subtotal,
                        ItemNotd = itemDto.Note
                    });
                }

                decimal finalAmount = totalAmount - (totalAmount * (requestDto.DiscountAmount ?? 0) / 100);
                if (finalAmount < 0) finalAmount = 0;

                var order = new Order
                {
                    UserId = requestDto.UserId,
                    CustomerName = requestDto.CustomerName,
                    Customer_phone = requestDto.Customer_phone,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending, 
                    Notes = requestDto.Notes,
                    TotalAmount = totalAmount,
                    DiscountAmount = requestDto.DiscountAmount ?? 0,
                    FinalAmount = finalAmount,
                    OrderItems = orderItemsEntities
                };

                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrderResponseDto
                {
                    order_id = order.OrderId,
                    order_date = order.OrderDate,
                    total_amount = order.TotalAmount,
                    discount_amount = order.DiscountAmount ?? 0,
                    final_amount = order.FinalAmount,
                    status = order.Status.ToString(),
                    notes = order.Notes,
                    user_id = order.UserId,
                    customer_name = order.CustomerName,
                    customer_phone = order.Customer_phone,
                    items = order.OrderItems?.Select(oi => new OrderItemResponseDto
                    {
                        order_item_id = oi.OrderItemId,
                        item_id = oi.ItemId,
                        quantity = oi.Quantity,
                        price_at_sale = oi.PriceAtSale,
                        subtotal = oi.Subtotal,
                        item_notd = oi.ItemNotd
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng, transaction rollback");
                throw;
            }
        }

        //  Xóa đơn hàng
        public async Task<bool> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Order
                    .Include(o => o.Payments)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng ID: {Id} để xóa.", id);
                    return false;
                }

                var orderItems = _context.OrderItem.Where(oi => oi.OrderId == id);
                _context.OrderItem.RemoveRange(orderItems);

                if (order.Payments != null && order.Payments.Any())
                    _context.Payments.RemoveRange(order.Payments);

                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã xóa đơn hàng ID {Id} thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa đơn hàng ID {Id}", id);
                throw;
            }
        }

        // Sửa đơn hàng
        public async Task<OrderResponseDto?> EditOrder(int id, OrderPostDto orderDto)
        {
            try
            {
                var order = await _context.Order.FindAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng ID {id}", id);
                    return null;
                }

                order.TotalAmount = orderDto.total_amount;
                order.DiscountAmount = orderDto.discount_amount;
                order.FinalAmount = orderDto.final_amount;

                //  Dùng Enum.Parse an toàn
                if (Enum.TryParse<OrderStatus>(orderDto.status, true, out var parsedStatus))
                    order.Status = parsedStatus;

                order.Notes = orderDto.notes;
                order.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã cập nhật đơn hàng ID {id} thành công", id);

                return new OrderResponseDto
                {
                    order_id = order.OrderId,
                    order_date = order.OrderDate,
                    total_amount = order.TotalAmount,
                    discount_amount = order.DiscountAmount ?? 0,
                    final_amount = order.FinalAmount,
                    status = order.Status.ToString(),
                    notes = order.Notes,
                    user_id = order.UserId,
                    customer_name = order.CustomerName,
                    created_at = order.CreatedAt,
                    updated_at = order.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật đơn hàng ID {id}", id);
                throw;
            }
        }

        //  Cập nhật trạng thái đơn hàng
        public async Task<bool> UpdateOrderStatus(int id, string newStatus)
        {
            try
            {
                var order = await _context.Order.FindAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng ID {id}", id);
                    return false;
                }

                if (!Enum.TryParse<OrderStatus>(newStatus, true, out var parsedStatus))
                {
                    _logger.LogWarning("Trạng thái '{status}' không hợp lệ", newStatus);
                    return false;
                }

                order.Status = parsedStatus;
                order.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã cập nhật trạng thái đơn hàng ID {id} thành {status}", id, newStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng ID {id}", id);
                throw;
            }
        }
    }
}
