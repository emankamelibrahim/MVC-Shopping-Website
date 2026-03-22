using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Data.Repositories.Interfaces;
using ShoppingWebsite.Models.ViewModels.Orders;
using ShoppingWebsite.Services.Interfaces;

namespace ShoppingWebsite.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly ICartRepo _cartRepo;
        private readonly IOrderRepo _orderRepo;

        public OrderService(
            AppDbContext context,
            ICartRepo cartRepo,
            IOrderRepo orderRepo)
        {
            _context = context;
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
        }

        public async Task<(bool Success, string Message, int OrderId)> CheckoutAsync(
            string userId,
            int shippingAddressId,
            NewAddressVM? newAddress)
        {
            // Begin transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ── 1. Resolve shipping address ────────────────
                int addressId = shippingAddressId;

                if (newAddress != null)
                {
                    var address = new Address
                    {
                        UserId = userId,
                        Country = newAddress.Country,
                        City = newAddress.City,
                        Street = newAddress.Street,
                        Zip = newAddress.Zip,
                        IsDefault = newAddress.IsDefault
                    };

                    if (newAddress.SaveAddress)
                    {
                        _context.Addresses.Add(address);
                        await _context.SaveChangesAsync();
                        addressId = address.AddressId;
                    }
                    else
                    {
                        // Temp address — save then use
                        _context.Addresses.Add(address);
                        await _context.SaveChangesAsync();
                        addressId = address.AddressId;
                    }
                }

                // ── 2. Get cart items ──────────────────────────
                var cartItems = await _cartRepo.GetCartItemsByUserAsync(userId);

                if (!cartItems.Any())
                    return (false, "Your cart is empty.", 0);

                // ── 3. Validate stock ──────────────────────────
                foreach (var item in cartItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null || !product.IsActive)
                        return (false, $"{item.Product.Name} is no longer available.", 0);

                    if (product.StockQuantity < item.Quantity)
                        return (false, $"Only {product.StockQuantity} units of {product.Name} are available.", 0);
                }

                // ── 4. Create order ────────────────────────────
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = addressId,
                    OrderNumber = GenerateOrderNumber(),
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity)
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // ── 5. Create order items + decrease stock ─────
                foreach (var item in cartItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);

                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        UnitPrice = product!.Price,
                        Quantity = item.Quantity,
                        LineTotal = product.Price * item.Quantity
                    });

                    product.StockQuantity -= item.Quantity;
                }

                await _context.SaveChangesAsync();

                // ── 6. Clear cart ──────────────────────────────
                await _cartRepo.ClearCartAsync(userId);
                await _context.SaveChangesAsync();

                // ── 7. Commit ──────────────────────────────────
                await transaction.CommitAsync();

                return (true, "Order placed successfully.", order.OrderId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Something went wrong: {ex.Message}", 0);
            }
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
            => await _orderRepo.GetOrdersByUserAsync(userId);

        public async Task<Order?> GetOrderDetailsAsync(int orderId, string userId)
        {
            var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
            if (order == null || order.UserId != userId)
                return null;
            return order;
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);

            if (order == null || order.UserId != userId)
                return false;

            if (order.Status != OrderStatus.Pending)
                return false;

            // Restore stock
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                    product.StockQuantity += item.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }

        private static string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp}-{random}";
        }
    }
}