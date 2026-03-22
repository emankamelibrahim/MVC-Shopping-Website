using System.ComponentModel.DataAnnotations;

namespace ShoppingWebsite.Models.ViewModels.Admin
{
    public class CategoryListVM
    {
        public IEnumerable<CategoryRowVM> Categories { get; set; } = new List<CategoryRowVM>();
    }

    public class CategoryRowVM
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ParentName { get; set; }
        public int ProductCount { get; set; }
    }

    public class CategoryFormVM
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? ParentCategoryId { get; set; }

        public IEnumerable<CategoryOptionVM> ParentOptions { get; set; } = new List<CategoryOptionVM>();
    }

    public class CategoryOptionVM
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}