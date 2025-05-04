using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class UserVM
    {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [Remote("IsUserNameExist", "User", ErrorMessage = "This {0} already exists", AdditionalFields = nameof(Id))]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} must be at least {2} characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "{0} must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
        [DisplayName("Password")]
        public string Password { get; set; }

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

        [DisplayName("Age")]
        public int Age => DateTime.Now.Year - BirthDate.Year;

        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid {0} address")]
        [Remote("IsEmailExist", "User", ErrorMessage = "This {0} already exists", AdditionalFields = nameof(Id))]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid {0} number")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid {0} number")]
        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Address")]
        public int AddressId { get; set; }

        public AddressVM? Address { get; set; }
        #endregion

        #region Methods
        public static UserVM FromUser(User user)
        {
            return new UserVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = string.Empty, // Don't expose password hash
                Role = user.Role,
                Gender = user.Gender,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AddressId = user.AddressId,
                Address = user.address != null ? AddressVM.FromAddress(user.address) : null
            };
        }

        public static User FromUserVM(UserVM userVM)
        {
            return new User
            {
                Id = userVM.Id,
                UserName = userVM.UserName,
                Role = userVM.Role,
                Gender = userVM.Gender,
                FirstName = userVM.FirstName,
                LastName = userVM.LastName,
                BirthDate = userVM.BirthDate,
                Email = userVM.Email,
                PhoneNumber = userVM.PhoneNumber,
                AddressId = userVM.AddressId
            };
        }
        #endregion
    }
}