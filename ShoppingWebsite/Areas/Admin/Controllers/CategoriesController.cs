using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Models.ViewModels.Admin;

namespace ShoppingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                    .ThenInclude(s => s.Products)
                .ToListAsync();

            var vm = new CategoryListVM
            {
                Categories = categories.Select(c => new CategoryRowVM
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ParentName = c.ParentCategory?.Name,
                    ProductCount = c.Products.Count
                                 + c.SubCategories.Sum(s => s.Products.Count)
                })
            };

            return View(vm);
        }

        // GET: /Admin/Categories/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CategoryFormVM
            {
                ParentOptions = await GetParentOptionsAsync()
            };
            return View("Form", vm);
        }

        // POST: /Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ParentOptions = await GetParentOptionsAsync();
                return View("Form", model);
            }

            var category = new Category
            {
                Name = model.Name,
                ParentCategoryId = model.ParentCategoryId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Category '{category.Name}' created successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/Categories/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var vm = new CategoryFormVM
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                ParentOptions = await GetParentOptionsAsync(excludeId: id)
            };

            return View("Form", vm);
        }

        // POST: /Admin/Categories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ParentOptions = await GetParentOptionsAsync(excludeId: id);
                return View("Form", model);
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.Name = model.Name;
            category.ParentCategoryId = model.ParentCategoryId;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Category '{category.Name}' updated successfully.";
            return RedirectToAction("Index");
        }

        // POST: /Admin/Categories/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();

            if (category.Products.Any() || category.SubCategories.Any())
            {
                TempData["Error"] = "Cannot delete a category that has products or subcategories.";
                return RedirectToAction("Index");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Category '{category.Name}' deleted.";
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<CategoryOptionVM>> GetParentOptionsAsync(int? excludeId = null)
        {
            var query = _context.Categories
                .Where(c => c.ParentCategoryId == null);

            if (excludeId.HasValue)
                query = query.Where(c => c.CategoryId != excludeId.Value);

            return await query
                .Select(c => new CategoryOptionVM
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}