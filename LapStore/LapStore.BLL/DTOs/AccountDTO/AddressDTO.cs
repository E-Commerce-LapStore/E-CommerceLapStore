using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class AddressDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(100, ErrorMessage = "Street cannot exceed 100 characters")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "State/Governorate is required")]
        [StringLength(50, ErrorMessage = "State/Governorate cannot exceed 50 characters")]
        public string Governorate { get; set; }

        [Required(ErrorMessage = "Postal/ZIP code is required")]
        [StringLength(10, ErrorMessage = "Postal/ZIP code cannot exceed 10 characters")]
        [DisplayName("ZIP Code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string Country { get; set; }

        // Navigation Properties
        public virtual UserDTO? User { get; set; }

        // Conversion methods
        public static AddressDTO FromAddress(Address address)
        {
            return new AddressDTO
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                Governorate = address.Governorate,
                ZipCode = address.ZipCode,
                Country = address.Country
            };
        }

        public static Address FromAddressVM(AddressDTO addressVM)
        {
            return new Address
            {
                Id = addressVM.Id,
                Street = addressVM.Street,
                City = addressVM.City,
                Governorate = addressVM.Governorate,
                ZipCode = addressVM.ZipCode,
                Country = addressVM.Country
            };
        }
    }
}