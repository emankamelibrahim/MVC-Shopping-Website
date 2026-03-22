using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Data.Repositories.Interfaces;

namespace ShoppingWebsite.Data.Repositories.Implementations
{
    public class OrderRepo : EntityRepo<Order>, IOrderRepo
    {
        public OrderRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId)
            => await _dbSet
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
            => await _dbSet
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

        public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
            => await _dbSet
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }
}