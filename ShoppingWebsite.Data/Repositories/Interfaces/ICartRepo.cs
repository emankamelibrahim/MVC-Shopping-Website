using ShoppingWebsite.Data.Entities;

namespace ShoppingWebsite.Data.Repositories.Interfaces
{
    public interface ICartRepo : IEntityRepo<CartItem>
    {
        Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(string userId);
        Task<CartItem?> GetCartItemAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
    }
}