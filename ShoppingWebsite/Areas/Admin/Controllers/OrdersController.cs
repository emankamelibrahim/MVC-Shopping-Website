using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Data.Context;
using ShoppingWebsite.Data.Entities;
using ShoppingWebsite.Models.ViewModels.Admin;
using ShoppingWebsite.Models.ViewModels.Orders;

namespace ShoppingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Orders
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var parsedStatus))
                query = query.Where(o => o.Status == parsedStatus);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var vm = new AdminOrderListVM
            {
                Orders = orders.Select(o => new AdminOrderRowVM
                {
                    OrderId = o.OrderId,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.User.FullName,
                    CustomerEmail = o.User.Email!,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    ItemCount = o.OrderItems.Count,
                    Status = o.Status,
                    StatusDisplay = o.Status.ToString(),
                    StatusClass = GetStatusClass(o.Status)
                }),
                SelectedStatus = status
            };

            return View(vm);
        }

        // GET: /Admin/Orders/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            var vm = new AdminOrderDetailsVM
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                CustomerName = order.User.FullName,
                CustomerEmail = order.User.Email!,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                StatusDisplay = order.Status.ToString(),
                StatusClass = GetStatusClass(order.Status),
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

        // POST: /Admin/Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) return NotFound();

            // Restore stock if cancelling
            if (status == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                        product.StockQuantity += item.Quantity;
                }
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order status updated to {status}.";
            return RedirectToAction("Details", new { id = orderId });
        }

        private static string GetStatusClass(OrderStatus status) => status switch
        {
            OrderStatus.Pending => "sw-status-pending",
            OrderStatus.Processing => "sw-status-processing",
            OrderStatus.Shipped => "sw-status-shipped",
            OrderStatus.Delivered => "sw-status-delivered",
            OrderStatus.Cancelled => "sw-status-cancelled",
            _ => ""
        };
    }
}