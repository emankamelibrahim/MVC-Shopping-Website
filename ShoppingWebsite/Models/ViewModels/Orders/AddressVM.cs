using System.ComponentModel.DataAnnotations;

namespace ShoppingWebsite.Models.ViewModels.Orders
{
    public class AddressVM
    {
        public int AddressId { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string Display => $"{Street}, {City}, {Country} {Zip}";
    }

    public class NewAddressVM
    {
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street is required.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zip code is required.")]
        public string Zip { get; set; } = string.Empty;

        public bool SaveAddress { get; set; } = true;
        public bool IsDefault { get; set; }
    }
}