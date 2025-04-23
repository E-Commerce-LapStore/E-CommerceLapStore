using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.Web.ViewModels
{
    public class UserVM
    {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [Remote("IsUserNameExist", "User", ErrorMessage = "This Username already exists", AdditionalFields = nameof(Id))]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select your role")]
        [DisplayName("Role")]
        public UserRole Role { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [DisplayName("Gender")]
        public UserGender Gender { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Birth date is required")]
        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

        [DisplayName("Age")]
        public int Age => DateTime.Now.Year - BirthDate.Year;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Remote("IsEmailExist", "User", ErrorMessage = "This Email already exists", AdditionalFields = nameof(Id))]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid phone number")]
        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [DisplayName("Address")]
        public int AddressId { get; set; }
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
                AddressId = user.AddressId
            };
        }

        public static User FromUserVM(UserVM userVM)
        {
            return new User
            {
                Id = userVM.Id,
                UserName = userVM.UserName,
                PasswordHash = string.Empty, // Don't expose password hash
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