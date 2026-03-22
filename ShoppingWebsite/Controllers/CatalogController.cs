using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Repositories.Interfaces;
using ShoppingWebsite.Models.ViewModels.Catalog;
using Microsoft.EntityFrameworkCore;

namespace ShoppingWebsite.Controllers
{
    [Authorize]
    public class CatalogController : Controller
    {
        private readonly IProductRepo _productRepo;
        private readonly AppDbContext _context;

        public CatalogController(IProductRepo productRepo, AppDbContext context)
        {
            _productRepo = productRepo;
            _context = context;
        }

        public async Task<IActionResult> Index(
     int? categoryId, string? q, string? sort, int page = 1)
        {
            const int pageSize = 12;

            var products = await _productRepo.GetFilteredProductsAsync(
                categoryId, q, sort, page, pageSize);

            var totalCount = await _productRepo.GetFilteredProductsCountAsync(categoryId, q);

            var allProductsCount = await _productRepo.GetFilteredProductsCountAsync(null, null);

            var categories = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                    .ThenInclude(s => s.Products)
                .ToListAsync();

            var vm = new ProductListVM
            {
                Products = products.Select(p => new ProductCardVM
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryName = p.Category.Name
                }),
                SelectedCategoryId = categoryId,
                Search = q,
                Sort = sort,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalCount = totalCount,
                PageSize = pageSize,
                AllProductsCount = allProductsCount,
                Categories = categories.Select(c => new CategoryVM
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId,
                    // parent count = own products + all subcategory products
                    ProductCount = c.Products.Count(p => p.IsActive)
                                     + c.SubCategories.Sum(s => s.Products.Count(p => p.IsActive))
                })
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepo.GetProductWithCategoryAsync(id);

            if (product == null || !product.IsActive)
                return NotFound();

            var vm = new ProductDetailsVM
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryName = product.Category.Name,
                CategoryId = product.CategoryId,
                SKU = product.SKU
            };

            return View(vm);
        }
    }
}