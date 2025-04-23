using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LapStore.Web.ViewModels
{
    public class AddressVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Street address is required")]
        [StringLength(100, ErrorMessage = "Street address cannot exceed 100 characters")]
        [DisplayName("Street Address")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        [DisplayName("City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        [DisplayName("State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        [DisplayName("Country")]
        public string Country { get; set; }

        // Navigation Properties
        public virtual UserVM? User { get; set; }

        // Conversion methods
        public static AddressVM FromAddress(Address address)
        {
            return new AddressVM
            {
                Id = address.Id,
                StreetAddress = address.Street,
                City = address.City,
                State = address.Governorate,
                PostalCode = address.ZipCode,
                Country = address.Country
            };
        }

        public static Address FromAddressVM(AddressVM addressVM)
        {
            return new Address
            {
                Id = addressVM.Id,
                Street = addressVM.StreetAddress,
                City = addressVM.City,
                Governorate = addressVM.State,
                ZipCode = addressVM.PostalCode,
                Country = addressVM.Country,
            };
        }
    }
} 