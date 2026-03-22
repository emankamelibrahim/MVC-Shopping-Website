using ShoppingWebsite.Data.Entities;

namespace ShoppingWebsite.Models.ViewModels.Orders
{
    public class OrderDetailsVM
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
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

        // Shipping
        public string ShippingAddress { get; set; } = string.Empty;

        // Items
        public IEnumerable<OrderItemVM> Items { get; set; } = new List<OrderItemVM>();
    }

    public class OrderItemVM
    {
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}