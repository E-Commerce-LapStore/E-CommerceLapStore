using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using LapStore.BLL.Validations;

namespace LapStore.BLL.ViewModels
{

    public class ProductVM
    {
        #region Properties
        public int Id { get; set; }
        
        [Required(ErrorMessage = "{0} is required")]
        [Remote("IsProductNameExist", "Product", ErrorMessage = "This {0} already exists", AdditionalFields = nameof(Id))]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "{0} must be greater than {1}")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "{0} must be greater than {1}")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public string? CategoryName { get; set; }

        [RequiredForNewProduct(ErrorMessage = "{0} is required for new products")]
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