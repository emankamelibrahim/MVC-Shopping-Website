using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Data.Repositories.Interfaces;

namespace ShoppingWebsite.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly ICartRepo _cartRepo;
        private readonly UserManager<AppUser> _userManager;

        public CartCountViewComponent(ICartRepo cartRepo, UserManager<AppUser> userManager)
        {
            _cartRepo = cartRepo;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity!.IsAuthenticated)
                return Content("0");

            var userId = _userManager.GetUserId(HttpContext.User)!;
            var items = await _cartRepo.GetCartItemsByUserAsync(userId);
            var count = items.Sum(i => i.Quantity);

            return Content(count.ToString());
        }
    }
}