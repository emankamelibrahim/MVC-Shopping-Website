using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Data.Repositories.Interfaces;

namespace ShoppingWebsite.Data.Repositories.Implementations
{
    public class ProductRepo : EntityRepo<Product>, IProductRepo
    {
        public ProductRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetFilteredProductsAsync(
            int? categoryId, string? search, string? sort, int page, int pageSize)
        {
            var query = _dbSet
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value
                    || p.Category.ParentCategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search)
                    || p.Description!.Contains(search));

            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetFilteredProductsCountAsync(int? categoryId, string? search)
        {
            var query = _dbSet.Where(p => p.IsActive);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value
                    || p.Category.ParentCategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search)
                    || p.Description!.Contains(search));

            return await query.CountAsync();
        }

        public async Task<Product?> GetProductWithCategoryAsync(int id)
            => await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
    }
}