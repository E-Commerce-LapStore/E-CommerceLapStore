using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [DisplayName("Username")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} must be at least {2} characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])[A-Za-z\d\W_]{8,}$",
            ErrorMessage = "{0} must contain at least one uppercase letter, one lowercase letter, one number and one non-alphanumeric character")]
        [DisplayName("Password")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your {0}")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The {0} and confirmation {0} do not match.")]
        [DisplayName("Confirm Password")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid {0} address")]
        [DisplayName("Email")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayName("First Name")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayName("Last Name")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Gender")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public UserGender Gender { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Phone(ErrorMessage = "Invalid {0}")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid {0} with the country code")]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        // Address Fields
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string Street { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string City { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string Governorate { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayName("ZIP Code")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
        [DisplayFormat(NullDisplayText = "Not specified")]
        public string Country { get; set; }


    }
}