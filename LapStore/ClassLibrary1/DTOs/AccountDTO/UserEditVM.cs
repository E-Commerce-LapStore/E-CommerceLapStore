using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class UserEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [Remote("IsUserNameExist", "User", ErrorMessage = "This {0} already exists", AdditionalFields = nameof(Id))]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please select your {0}")]
        [DisplayName("Role")]
        public UserRole Role { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Gender")]
        public UserGender Gender { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid {0} address")]
        [Remote("IsEmailExist", "User", ErrorMessage = "This {0} already exists", AdditionalFields = nameof(Id))]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid {0}")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid {0} with the country code")]
        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Address")]
        public int AddressId { get; set; }

        public AddressVM? Address { get; set; }

        // Mapping methods for convenience
        public static UserEditVM FromUser(User user)
        {
            return new UserEditVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role,
                Gender = user.Gender,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AddressId = user.AddressId,
                Address = user.address != null ? AddressVM.FromAddress(user.address) : new AddressVM()
            };
        }

        public static void UpdateUserFromEditVM(UserEditVM vm, User user)
        {
            user.UserName = vm.UserName;
            user.Role = vm.Role;
            user.Gender = vm.Gender;
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.BirthDate = vm.BirthDate;
            user.Email = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;
            // Update address fields if present
            if (user.address != null && vm.Address != null)
            {
                user.address.Street = vm.Address.Street;
                user.address.City = vm.Address.City;
                user.address.Governorate = vm.Address.Governorate;
                user.address.ZipCode = vm.Address.ZipCode;
                user.address.Country = vm.Address.Country;
            }
        }
    }
}