using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Hosting;

namespace ShoppingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var vm = new ProductListAdminVM
            {
                Products = products.Select(p => new ProductRowVM
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    SKU = p.SKU,
                    CategoryName = p.Category.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    IsActive = p.IsActive
                })
            };

            return View(vm);
        }

        // GET: /Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            var vm = new ProductFormVM
            {
                CategoryOptions = await GetCategoryOptionsAsync()
            };
            return View("Form", vm);
        }

        // POST: /Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormVM model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryOptions = await GetCategoryOptionsAsync();
                return View("Form", model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile);

            var product = new Product
            {
                Name = model.Name,
                SKU = model.SKU,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Description = model.Description,
                ImageUrl = imageUrl ?? model.ImageUrl,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' created successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/Products/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var vm = new ProductFormVM
            {
                ProductId = product.ProductId,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                CategoryOptions = await GetCategoryOptionsAsync()
            };

            return View("Form", vm);
        }

        // POST: /Admin/Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormVM model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryOptions = await GetCategoryOptionsAsync();
                return View("Form", model);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var imageUrl = await SaveImageAsync(model.ImageFile);

            product.Name = model.Name;
            product.SKU = model.SKU;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.Description = model.Description;
            product.ImageUrl = imageUrl ?? model.ImageUrl ?? product.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' updated successfully.";
            return RedirectToAction("Index");
        }

        // POST: /Admin/Products/ToggleActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = !product.IsActive;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' {(product.IsActive ? "activated" : "deactivated")}.";
            return RedirectToAction("Index");
        }

        // POST: /Admin/Products/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.OrderItems)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            if (product.OrderItems.Any())
            {
                // Soft delete — has order history
                product.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Product '{product.Name}' deactivated (has order history).";
            }
            else
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Product '{product.Name}' deleted.";
            }

            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<CategoryOptionVM>> GetCategoryOptionsAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == null)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var result = new List<CategoryOptionVM>();

            foreach (var parent in categories)
            {
                result.Add(new CategoryOptionVM
                {
                    CategoryId = parent.CategoryId,
                    Name = parent.Name
                });

                foreach (var sub in parent.SubCategories.OrderBy(s => s.Name))
                {
                    result.Add(new CategoryOptionVM
                    {
                        CategoryId = sub.CategoryId,
                        Name = $"  └ {sub.Name}"
                    });
                }
            }

            return result;
        }
        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                return null;

            var folder = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/images/products/{fileName}";
        }
    }
}