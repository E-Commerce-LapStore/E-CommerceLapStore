using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LapStore.Web.ViewModels
{
    public class ProductVM
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0.01,Double.MaxValue, ErrorMessage= "Min value equals 0.01")]
        public decimal Price { get; set; }

        [Range(0.01, Double.MaxValue, ErrorMessage = "Min value equals 0.01")]
        public decimal Weight { get; set; }
        public int CategoryId { get; set; }
        #endregion

        #region Methods

        public static ProductVM FromProduct(Product product)
        {
            return new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Weight = product.Weight,
                CategoryId = product.CategoryId,

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
                CategoryId = productVM.CategoryId,

            };
        }
        #endregion
    }
}
