using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Models.ViewModels.Orders;

namespace ShoppingWebsite.Services.Interfaces
{
    public interface IOrderService
    {
        Task<(bool Success, string Message, int OrderId)> CheckoutAsync(
            string userId,
            int shippingAddressId,
            NewAddressVM? newAddress);

        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderDetailsAsync(int orderId, string userId);
        Task<bool> CancelOrderAsync(int orderId, string userId);
    }
}