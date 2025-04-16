using LapStore.DAL.Repositories;
using LapStore.DAL.Data.Contexts;

namespace LapStore.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LapStoreDbContext _context;

        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public UnitOfWork(LapStoreDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context);
        }

        public IGenericRepository<T> GenericRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as IGenericRepository<T>;
            }
            var repository = new GenericRepository<T>(_context);
            _repositories[typeof(T)] = repository;
            return repository;
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        
    }
}
