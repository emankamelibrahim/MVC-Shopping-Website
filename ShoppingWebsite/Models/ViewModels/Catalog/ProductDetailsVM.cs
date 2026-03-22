namespace ShoppingWebsite.Models.ViewModels.Catalog
{
    public class ProductDetailsVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public bool IsInStock => StockQuantity > 0;
    }
}