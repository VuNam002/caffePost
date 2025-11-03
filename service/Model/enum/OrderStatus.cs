namespace CaffePOS.Model.Enums
{
    public enum OrderStatus
    {
        Pending,        // Đang chờ xác nhận
        Processing,     // Đang xử lý
        Completed,      // Hoàn thành
        Cancelled,      // Đã hủy
        Refunded,        // Đã hoàn tiền
        Paid
    }
}
