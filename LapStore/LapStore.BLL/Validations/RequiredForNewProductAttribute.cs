using LapStore.BLL.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.Validations
{
    public class RequiredForNewProductAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var productVM = (ProductVM)validationContext.ObjectInstance;

            if (productVM.Id == 0 && value == null) // New product (Id = 0) and no file
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}
