using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LapStore.DAL.Data.Entities;

namespace LapStore.Web.ViewModels
{
    public class ProductImageVM
    {
        #region Properties
        public int Id { get; set; }
        
        public string URL { get; set; }
        public int ProductId { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "This Field is required")]
        public FormFile? File { get; set; }
        #endregion

        #region Methods
        public static ProductImageVM FromProductImage(ProductImage productImage)
        {
            return new ProductImageVM
            {
                Id = productImage.ProductId,
                URL = productImage.URL,
                ProductId = productImage.Id
            };
        }
        public static ProductImage FromProductImageVM(ProductImageVM productImageVM)
        {
            return new ProductImage
            {
                Id = productImageVM.Id,
                URL = productImageVM.URL,
                ProductId = productImageVM.ProductId
            };
        }
        #endregion
    }
}
