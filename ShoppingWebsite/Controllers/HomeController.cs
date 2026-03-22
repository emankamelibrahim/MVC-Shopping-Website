using Microsoft.AspNetCore.Mvc;
using ShoppingWebsite.Data.Repositories.Interfaces;

namespace ShoppingWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepo _productRepo;

        public HomeController(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productRepo.GetFilteredProductsAsync(
                categoryId: null,
                search: null,
                sort: "newest",
                page: 1,
                pageSize: 8);

            return View(featuredProducts);
        }
    }
}