using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CaffePOS.Model.Enums;

namespace CaffePOS.Model
{
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("customer_name")]
        public string CustomerName { get; set; }

        [Column("discount_amount")]
        public decimal? DiscountAmount { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("status")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column("final_amount")]
        public decimal FinalAmount { get; set; }

        [Column("order_date")]
        public DateTime? OrderDate { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("customer_phone")]
        public string? Customer_phone { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Thuộc tính điều hướng đến người dùng đã tạo đơn hàng
        public virtual Users? User { get; set; }

        // Thuộc tính điều hướng: Một đơn hàng có nhiều chi tiết đơn hàng (OrderItem)
        public virtual ICollection<OrderItem>? OrderItems { get; set; }

        // Thuộc tính điều hướng: Một đơn hàng có thể có nhiều giao dịch thanh toán
        public virtual ICollection<Payments>? Payments { get; set; }
    }
}