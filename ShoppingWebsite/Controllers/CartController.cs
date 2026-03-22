using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Data.Repositories.Interfaces;
using ShoppingWebsite.Models.ViewModels.Cart;

namespace ShoppingWebsite.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepo _cartRepo;
        private readonly IProductRepo _productRepo;
        private readonly UserManager<AppUser> _userManager;

        public CartController(
            ICartRepo cartRepo,
            IProductRepo productRepo,
            UserManager<AppUser> userManager)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userManager = userManager;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var items = await _cartRepo.GetCartItemsByUserAsync(userId);

            var vm = new CartVM
            {
                Items = items.Select(i => new CartItemVM
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

            return View(vm);
        }

        // POST: /Cart/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User)!;
            var product = await _productRepo.GetByIdAsync(productId);

            if (product == null || !product.IsActive)
                return NotFound();

            var existingItem = await _cartRepo.GetCartItemAsync(userId, productId);

            if (existingItem != null)
            {
                // Update quantity but don't exceed stock
                var newQty = existingItem.Quantity + quantity;
                existingItem.Quantity = Math.Min(newQty, product.StockQuantity);
                _cartRepo.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = Math.Min(quantity, product.StockQuantity),
                    AddedAt = DateTime.UtcNow
                };
                await _cartRepo.AddAsync(cartItem);
            }

            await _cartRepo.SaveChangesAsync();

            TempData["Success"] = $"{product.Name} added to cart.";
            return RedirectToAction("Details", "Catalog", new { id = productId });
        }

        // POST: /Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int cartItemId, int quantity)
        {
            var userId = _userManager.GetUserId(User)!;
            var item = await _cartRepo.GetByIdAsync(cartItemId);

            if (item == null || item.UserId != userId)
                return NotFound();

            if (quantity <= 0)
            {
                _cartRepo.Delete(item);
            }
            else
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                item.Quantity = Math.Min(quantity, product!.StockQuantity);
                _cartRepo.Update(item);
            }

            await _cartRepo.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var userId = _userManager.GetUserId(User)!;
            var item = await _cartRepo.GetByIdAsync(cartItemId);

            if (item == null || item.UserId != userId)
                return NotFound();

            _cartRepo.Delete(item);
            await _cartRepo.SaveChangesAsync();

            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }
    }
}