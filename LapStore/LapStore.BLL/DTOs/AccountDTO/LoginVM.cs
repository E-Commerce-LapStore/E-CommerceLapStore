using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username is required")]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me")]
        public bool RememberMe { get; set; }
    }
}