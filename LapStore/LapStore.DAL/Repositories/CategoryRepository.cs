using LapStore.DAL.Data.Entities;
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
        public CategoryRepository(DbContext context) : base(context)
        {
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public override void Delete(Category category)
        {

            // Check if the category has any products
            // If category.products is null, use an empty collection instead
            if ((category.products ?? Enumerable.Empty<Product>()).Any())
            {
                // If there are any products, throw an exception
                throw new InvalidOperationException("Cannot delete a category that has products. Please ensure the category is empty before attempting to delete it.");
            }

            // If there are no products, proceed with the deletion
            base.Delete(category);
        }



    }

}
