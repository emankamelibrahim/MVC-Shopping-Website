using ShoppingWebsite.Models.ViewModels.Cart;

namespace ShoppingWebsite.Models.ViewModels.Orders
{
    public class CheckoutVM
    {
        // Cart summary (read-only display)
        public CartVM Cart { get; set; } = new();

        // Existing addresses to pick from
        public IEnumerable<AddressVM> SavedAddresses { get; set; } = new List<AddressVM>();

        // Selected existing address
        public int? SelectedAddressId { get; set; }

        // Or enter a new one
        public NewAddressVM NewAddress { get; set; } = new();

        // Toggle — true = use new address form
        public bool UseNewAddress { get; set; }
    }
}