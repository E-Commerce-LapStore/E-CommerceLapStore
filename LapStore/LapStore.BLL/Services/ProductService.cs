using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }
        public Product GetById(int? id)
        {
            return _productRepository.GetById(id);
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            _ = _unitOfWork.CompleteAsync();

        }
        public void Update(Product product)
        {
            _productRepository.Update(product);
            _unitOfWork.CompleteAsync();

        }
        public void Delete(Product product)
        {
            _productRepository.Delete(product);
            _unitOfWork.CompleteAsync();

        }
    }
} 