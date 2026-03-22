using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Data.Repositories.Interfaces;
using ShoppingWebsite.Models.ViewModels.Cart;
using ShoppingWebsite.Models.ViewModels.Orders;
using ShoppingWebsite.Services.Interfaces;

namespace ShoppingWebsite.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartRepo _cartRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly Data.Context.AppDbContext _context;

        public OrdersController(
            IOrderService orderService,
            ICartRepo cartRepo,
            UserManager<AppUser> userManager,
            Data.Context.AppDbContext context)
        {
            _orderService = orderService;
            _cartRepo = cartRepo;
            _userManager = userManager;
            _context = context;
        }

        // GET: /Orders
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var orders = await _orderService.GetUserOrdersAsync(userId);

            var vm = new OrderListVM
            {
                Orders = orders.Select(o => new OrderSummaryVM
                {
                    OrderId = o.OrderId,
                    OrderNumber = o.OrderNumber,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    ItemCount = o.OrderItems.Count
                })
            };

            return View(vm);
        }

        // GET: /Orders/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var order = await _orderService.GetOrderDetailsAsync(id, userId);

            if (order == null)
                return NotFound();

            var vm = new OrderDetailsVM
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress.Display,
                Items = order.OrderItems.Select(i => new OrderItemVM
                {
                    ProductName = i.Product.Name,
                    ImageUrl = i.Product.ImageUrl,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                })
            };

            return View(vm);
        }

        // GET: /Orders/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User)!;
            var cartItems = await _cartRepo.GetCartItemsByUserAsync(userId);

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var addresses = await GetUserAddressesAsync(userId);

            var vm = new CheckoutVM
            {
                Cart = new CartVM
                {
                    Items = cartItems.Select(i => new CartItemVM
                    {
                        CartItemId = i.CartItemId,
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        ImageUrl = i.Product.ImageUrl,
                        UnitPrice = i.Product.Price,
                        Quantity = i.Quantity,
                        MaxQuantity = i.Product.StockQuantity
                    })
                },
                SavedAddresses = addresses
            };

            return View(vm);
        }

        // POST: /Orders/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            var userId = _userManager.GetUserId(User)!;
            var addresses = await GetUserAddressesAsync(userId);

            // If user has no saved addresses, force new address mode
            if (!addresses.Any())
                model.UseNewAddress = true;

            // Validate address selection
            if (!model.UseNewAddress && model.SelectedAddressId == null)
            {
                ModelState.AddModelError("", "Please select a shipping address.");
                return await ReloadCheckout(model, userId);
            }

            if (model.UseNewAddress && !TryValidateModel(model.NewAddress))
                return await ReloadCheckout(model, userId);

            var result = await _orderService.CheckoutAsync(
                userId,
                model.SelectedAddressId ?? 0,
                model.UseNewAddress ? model.NewAddress : null);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return await ReloadCheckout(model, userId);
            }

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Details", new { id = result.OrderId });
        }

        // POST: /Orders/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var userId = _userManager.GetUserId(User)!;
            var success = await _orderService.CancelOrderAsync(orderId, userId);

            TempData[success ? "Success" : "Error"] = success
                ? "Order cancelled successfully."
                : "This order cannot be cancelled.";

            return RedirectToAction("Details", new { id = orderId });
        }

        // ── Helpers ────────────────────────────────────────────
        private async Task<IEnumerable<AddressVM>> GetUserAddressesAsync(string userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return addresses.Select(a => new AddressVM
            {
                AddressId = a.AddressId,
                Country = a.Country,
                City = a.City,
                Street = a.Street,
                Zip = a.Zip,
                IsDefault = a.IsDefault
            });
        }

        private async Task<IActionResult> ReloadCheckout(CheckoutVM model, string userId)
        {
            var cartItems = await _cartRepo.GetCartItemsByUserAsync(userId);
            model.Cart = new CartVM
            {
                Items = cartItems.Select(i => new CartItemVM
                {
                    CartItemId = i.CartItemId,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ImageUrl = i.Product.ImageUrl,
                    UnitPrice = i.Product.Price,
                    Quantity = i.Quantity,
                    MaxQuantity = i.Product.StockQuantity
                })
            };
            model.SavedAddresses = await GetUserAddressesAsync(userId);
            return View(model);
        }
    }
}