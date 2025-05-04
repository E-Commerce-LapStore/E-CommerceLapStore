using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();




        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategory(Category category);
        Task<bool> IsCategoryNameExistAsync(string categoryName, int? categoryId = null);
    }
} 