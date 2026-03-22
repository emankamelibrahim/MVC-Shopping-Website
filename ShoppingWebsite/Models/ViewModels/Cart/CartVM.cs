namespace ShoppingWebsite.Models.ViewModels.Cart
{
    public class CartVM
    {
        public IEnumerable<CartItemVM> Items { get; set; } = new List<CartItemVM>();
        public decimal Total => Items.Sum(i => i.LineTotal);
        public int ItemCount => Items.Sum(i => i.Quantity);
    }

    public class CartItemVM
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}