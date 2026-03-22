using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingWebsite.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
