using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingWebsite.Data.Entities
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public AppUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
