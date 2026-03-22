using ShoppingWebsite.Data.Entities;

namespace ShoppingWebsite.Models.ViewModels.Admin
{
    public class AdminOrderListVM
    {
        public IEnumerable<AdminOrderRowVM> Orders { get; set; } = new List<AdminOrderRowVM>();
        public string? SelectedStatus { get; set; }
    }

    public class AdminOrderRowVM
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public string StatusClass { get; set; } = string.Empty;
    }

    public class AdminOrderDetailsVM
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public string StatusClass { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public IEnumerable<ShoppingWebsite.Models.ViewModels.Orders.OrderItemVM> Items { get; set; }
            = new List<ShoppingWebsite.Models.ViewModels.Orders.OrderItemVM>();
    }
}