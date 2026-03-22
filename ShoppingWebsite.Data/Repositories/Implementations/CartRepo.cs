using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Data.Repositories.Interfaces;

namespace ShoppingWebsite.Data.Repositories.Implementations
{
    public class CartRepo : EntityRepo<CartItem>, ICartRepo
    {
        public CartRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(string userId)
            => await _dbSet
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

        public async Task<CartItem?> GetCartItemAsync(string userId, int productId)
            => await _dbSet
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        public async Task ClearCartAsync(string userId)
        {
            var items = await _dbSet.Where(c => c.UserId == userId).ToListAsync();
            _dbSet.RemoveRange(items);
        }
    }
}