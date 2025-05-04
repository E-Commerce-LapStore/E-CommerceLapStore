using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.DTOs
{
    public class ProductDTO
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
        [Range(0.01, Double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Weight must be a number with up to 2 decimal places")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public string? CategoryName { get; set; }

        
        [DisplayName("Main Product Image")]
        public IFormFile? MainImageFile { get; set; }

        [DisplayName("Additional Product Images")]
        public List<IFormFile>? AdditionalImageFiles { get; set; }

        public string? MainImageUrl { get; set; }

        public List<string>? AdditionalImageUrls { get; set; }

        [DisplayName("Images to Delete")]
        public List<string>? ImagesToDelete { get; set; }
        #endregion

        #region Methods
        public static ProductDTO FromProduct(Product product)
        {
            if (product == null)
                return null;

            var mainImage = product.productImages?.FirstOrDefault(pi => pi.IsMain);
            var additionalImages = product.productImages?.Where(pi => !pi.IsMain).ToList();

            return new ProductDTO
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

        public static Product FromProductDTO(ProductDTO productDTO)
        {
            if (productDTO == null)
                return null;

            var product = new Product
            {
                Id = productDTO.Id,
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Weight = Math.Round(productDTO.Weight, 2), // Round to 2 decimal places
                CategoryId = productDTO.CategoryId
            };

            // Preserve existing images if not being updated
            if (productDTO.Id > 0)
            {
                product.productImages = new List<ProductImage>();
                
                // Add main image if it exists and not being deleted
                if (!string.IsNullOrEmpty(productDTO.MainImageUrl) && 
                    (productDTO.ImagesToDelete == null || !productDTO.ImagesToDelete.Contains(productDTO.MainImageUrl)))
                {
                    product.productImages.Add(new ProductImage
                    {
                        ProductId = productDTO.Id,
                        URL = productDTO.MainImageUrl,
                        IsMain = true
                    });
                }

                // Add additional images if they exist and not being deleted
                if (productDTO.AdditionalImageUrls != null)
                {
                    foreach (var url in productDTO.AdditionalImageUrls)
                    {
                        if (!string.IsNullOrEmpty(url) && 
                            (productDTO.ImagesToDelete == null || !productDTO.ImagesToDelete.Contains(url)))
                        {
                            product.productImages.Add(new ProductImage
                            {
                                ProductId = productDTO.Id,
                                URL = url,
                                IsMain = false
                            });
                        }
                    }
                }
            }

            return product;
        }
        #endregion
    }
}