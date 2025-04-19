using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace LapStore.Web.ViewModels
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

    public class ProductVM
    {
        #region Properties
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [Remote("IsProductNameExist", "Product", ErrorMessage = "This Product Name already exists", AdditionalFields = nameof(Id))]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public string? CategoryName { get; set; }

        [RequiredForNewProduct(ErrorMessage = "Main product image is required for new products")]
        [DisplayName("Main Product Image")]
        public IFormFile? MainImageFile { get; set; }

        [DisplayName("Additional Product Images")]
        public List<IFormFile>? AdditionalImageFiles { get; set; }

        public string? MainImageUrl { get; set; }
        public List<string>? AdditionalImageUrls { get; set; }
        #endregion

        #region Methods
        public static ProductVM FromProduct(Product product)
        {
            var mainImage = product.productImages?.FirstOrDefault(pi => pi.IsMain);
            var additionalImages = product.productImages?.Where(pi => !pi.IsMain).ToList();

            return new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Weight = product.Weight,
                CategoryId = product.CategoryId,
                CategoryName = product.category?.Name,
                MainImageUrl = mainImage?.URL,
                AdditionalImageUrls = additionalImages?.Select(pi => pi.URL).ToList() ?? new List<string>()
            };
        }

        public static Product FromProductVM(ProductVM productVM)
        {
            return new Product
            {
                Id = productVM.Id,
                Name = productVM.Name,
                Description = productVM.Description,
                Price = productVM.Price,
                Weight = productVM.Weight,
                CategoryId = productVM.CategoryId
            };
        }
        #endregion
    }
}