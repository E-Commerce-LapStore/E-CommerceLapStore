using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Product GetById(int? id);
        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task AddAsync(Product product);
        void Delete(Product product);
        void Update(Product product);
    }
} 