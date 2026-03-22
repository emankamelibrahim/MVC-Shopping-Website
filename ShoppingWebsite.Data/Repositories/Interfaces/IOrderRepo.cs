using ShoppingWebsite.Data.Entities;

namespace ShoppingWebsite.Data.Repositories.Interfaces
{
    public interface IOrderRepo : IEntityRepo<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
    }
}