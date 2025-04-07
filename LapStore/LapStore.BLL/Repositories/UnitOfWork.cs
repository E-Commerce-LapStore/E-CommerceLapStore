using LapStore.BLL.Interfaces;
using LapStore.DAL.Contexts;
using LapStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LapStoreDbContext _context;

        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        public UnitOfWork(LapStoreDbContext context)
        {
            _context = context;
        }

        public IBaseRepository<T> BaseRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as IBaseRepository<T>;
            }
            var repository = new BaseRepository<T>(_context);
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
