using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.DTOs
{
    public class ProductReadDTO
    {
        #region Properties
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Weight { get; set; }

        public int CategoryId { get; set; }

        
        #endregion

        #region Methods
        public static ProductReadDTO FromProduct(Product product)
        {
            if (product == null)
                return null;

            return new ProductReadDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Weight = product.Weight,
                CategoryId = product.CategoryId
            };
        }

        #endregion
    }
}