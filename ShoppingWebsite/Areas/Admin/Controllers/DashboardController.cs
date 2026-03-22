using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Models.ViewModels.Admin;

namespace ShoppingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");

            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(8)
                .ToListAsync();

            var vm = new DashboardVM
            {
                TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalCustomers = customers.Count,
                TotalRevenue = await _context.Orders
                                    .Where(o => o.Status != OrderStatus.Cancelled)
                                    .SumAsync(o => o.TotalAmount),
                PendingOrders = await _context.Orders
                                    .CountAsync(o => o.Status == OrderStatus.Pending),
                LowStockProducts = await _context.Products
                                    .CountAsync(p => p.IsActive && p.StockQuantity <= 5),
                RecentOrders = recentOrders.Select(o => new RecentOrderVM
                {
                    OrderId = o.OrderId,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.User.FullName,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    StatusDisplay = o.Status.ToString(),
                    StatusClass = o.Status switch
                    {
                        OrderStatus.Pending => "sw-status-pending",
                        OrderStatus.Processing => "sw-status-processing",
                        OrderStatus.Shipped => "sw-status-shipped",
                        OrderStatus.Delivered => "sw-status-delivered",
                        OrderStatus.Cancelled => "sw-status-cancelled",
                        _ => ""
                    }
                })
            };

            return View(vm);
        }
    }
}