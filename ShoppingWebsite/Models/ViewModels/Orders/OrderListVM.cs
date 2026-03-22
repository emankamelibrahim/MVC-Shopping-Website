using ShoppingWebsite.Data.Entities;

namespace ShoppingWebsite.Models.ViewModels.Orders
{
    public class OrderListVM
    {
        public IEnumerable<OrderSummaryVM> Orders { get; set; } = new List<OrderSummaryVM>();
    }

    public class OrderSummaryVM
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public int ItemCount { get; set; }
        public string StatusDisplay => Status.ToString();
        public string StatusClass => Status switch
        {
            OrderStatus.Pending => "sw-status-pending",
            OrderStatus.Processing => "sw-status-processing",
            OrderStatus.Shipped => "sw-status-shipped",
            OrderStatus.Delivered => "sw-status-delivered",
            OrderStatus.Cancelled => "sw-status-cancelled",
            _ => ""
        };
    }
}