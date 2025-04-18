using LapStore.DAL.Data.Entities;
using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(LapStoreDbContext context) : base(context)
        {
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }
        public async Task<bool> IsCategoryNameExistAsync(string categoryName)
        {
            return await _dbSet.AnyAsync(c => c.Name == categoryName);
        }
    }
}
