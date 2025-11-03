using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price_at_sale", TypeName = "decimal(18, 2)")]
        public decimal PriceAtSale { get; set; }

        [Column("subtotal", TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }

        [Column("item_notd")]
        public string? ItemNotd { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Nếu cần liên kết với Order hoặc Item, thêm navigation property:
        public virtual Order? Order { get; set; }
        public virtual Items? Item { get; set; }
    }
}