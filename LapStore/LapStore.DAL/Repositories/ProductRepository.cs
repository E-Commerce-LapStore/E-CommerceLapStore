using LapStore.DAL.Data.Entities;
using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LapStore.DAL.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {

        public ProductRepository(LapStoreDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.products
                .Include(p => p.productImages)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Product> GetProductByNameAsync(string name)
        {
            return await _context.products
                .Include(p => p.productImages)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> IsProductNameExistAsync(string productName)
        {
            return await _dbSet.AnyAsync(p => p.Name == productName);
        }

        public void RemoveProductImage(ProductImage image)
        {
            _context.productImages.Remove(image);
        }
    }
}
