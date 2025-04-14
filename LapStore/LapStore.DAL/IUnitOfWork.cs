using LapStore.DAL.Repositories;

namespace LapStore.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GenericRepository<T>() where T : class;
        Task<int> CompleteAsync();
    }
}
