using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductByNameAsync(string name);
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);

        Task AddProductAsync(Product product, IFormFile mainImageFile, List<IFormFile>? additionalImageFiles);
        Task UpdateProductAsync(Product product, IFormFile? mainImageFile, List<IFormFile>? additionalImageFiles, List<string>? imagesToDelete);
        Task DeleteProduct(Product product);
        Task<bool> IsProductNameExistAsync(string productName);
    }
} 