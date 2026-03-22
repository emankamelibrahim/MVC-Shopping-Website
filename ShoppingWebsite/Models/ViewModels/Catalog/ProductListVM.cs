namespace ShoppingWebsite.Models.ViewModels.Catalog
{
    public class ProductListVM
    {
        // Catalog grid
        public IEnumerable<ProductCardVM> Products { get; set; } = new List<ProductCardVM>();

        // Filters
        public int? SelectedCategoryId { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }

        // Sidebar
        public IEnumerable<CategoryVM> Categories { get; set; } = new List<CategoryVM>();

        public int AllProductsCount { get; set; }
    }

    public class ProductCardVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CategoryVM
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public int ProductCount { get; set; }
    }
}