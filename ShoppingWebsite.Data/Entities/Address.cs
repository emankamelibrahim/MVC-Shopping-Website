using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingWebsite.Data.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Display => $"{Street}, {City}, {Country} {Zip}";
        public bool IsDefault { get; set; }

        public AppUser User { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
