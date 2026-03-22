using ShoppingWebsite.Data.Entities;

namespace ShoppingWebsite.Data.Repositories.Interfaces
{
    public interface IProductRepo : IEntityRepo<Product>
    {
        Task<IEnumerable<Product>> GetFilteredProductsAsync(
            int? categoryId, string? search, string? sort, int page, int pageSize);
        Task<int> GetFilteredProductsCountAsync(int? categoryId, string? search);
        Task<Product?> GetProductWithCategoryAsync(int id);
    }
}