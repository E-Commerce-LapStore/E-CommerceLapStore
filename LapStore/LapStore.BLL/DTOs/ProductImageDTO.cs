using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.DTOs
{
    public class ProductImageDTO
    {
        #region Properties
        public int Id { get; set; }
        
        public string URL { get; set; }
        public int ProductId { get; set; }

        //[NotMapped]
        //[Required(ErrorMessage = "This Field is required")]
        //public FormFile? File { get; set; }
        #endregion

        #region Methods
        public static ProductImageDTO FromProductImage(ProductImage productImage)
        {
            return new ProductImageDTO
            {
                Id = productImage.ProductId,
                URL = productImage.URL,
                ProductId = productImage.Id
            };
        }
        public static ProductImage FromProductImageDTO(ProductImageDTO productImageDTO)
        {
            return new ProductImage
            {
                Id = productImageDTO.Id,
                URL = productImageDTO.URL,
                ProductId = productImageDTO.ProductId
            };
        }
        #endregion
    }
}
