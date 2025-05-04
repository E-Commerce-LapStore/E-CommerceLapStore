using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class ChangePasswordVM
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [DisplayName("Current Password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your new password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [DisplayName("Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}