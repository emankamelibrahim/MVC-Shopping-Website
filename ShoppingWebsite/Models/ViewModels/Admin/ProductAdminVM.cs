using System.ComponentModel.DataAnnotations;

namespace ShoppingWebsite.Models.ViewModels.Admin
{
    public class ProductListAdminVM
    {
        public IEnumerable<ProductRowVM> Products { get; set; } = new List<ProductRowVM>();
    }

    public class ProductRowVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductFormVM
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required.")]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 999999, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, 999999, ErrorMessage = "Stock must be 0 or more.")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public IEnumerable<CategoryOptionVM> CategoryOptions { get; set; } = new List<CategoryOptionVM>();

        public IFormFile? ImageFile { get; set; }
    }
}