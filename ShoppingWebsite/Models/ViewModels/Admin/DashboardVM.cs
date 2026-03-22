namespace ShoppingWebsite.Models.ViewModels.Admin
{
    public class DashboardVM
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockProducts { get; set; }
        public IEnumerable<RecentOrderVM> RecentOrders { get; set; } = new List<RecentOrderVM>();
    }

    public class RecentOrderVM
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string StatusClass { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
    }
}