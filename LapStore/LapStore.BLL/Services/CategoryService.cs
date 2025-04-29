using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using Microsoft.EntityFrameworkCore;

namespace LapStore.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, IFileService fileService, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _categoryRepository.GetCategoryByNameAsync(name);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            var strategy = _unitOfWork.Context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();
                    await _categoryRepository.AddAsync(category);
                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var strategy = _unitOfWork.Context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();
                    _categoryRepository.Update(category);
                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
        }

        public async Task DeleteCategory(Category category)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Check if the category has any products
                if ((category.products ?? Enumerable.Empty<Product>()).Any())
                {
                    throw new InvalidOperationException("Cannot delete a category that has products. Please ensure the category is empty before attempting to delete it.");
                }

                // Delete the physical image file first if it exists
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    bool fileDeleted = _fileService.DeletePhysicalFile(category.ImageUrl);
                    if (!fileDeleted)
                    {
                        // Log warning but continue with database deletion
                        // You might want to add proper logging here
                        System.Diagnostics.Debug.WriteLine($"Warning: Could not delete file {category.ImageUrl}");
                    }
                }

                // Then delete from database
                _categoryRepository.Delete(category);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> IsCategoryNameExistAsync(string categoryName, int? categoryId = null)
        {
            return await _categoryRepository.IsCategoryNameExistAsync(categoryName, categoryId);
        }
    }
} 