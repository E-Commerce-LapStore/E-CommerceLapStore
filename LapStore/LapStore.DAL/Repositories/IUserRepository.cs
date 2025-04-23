using LapStore.DAL.Data.Entities;

namespace LapStore.DAL.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUserNameAsync(string userName);
        Task<bool> IsEmailExistAsync(string email, int? userId = null);
        Task<bool> IsUserNameExistAsync(string userName, int? userId = null);
        Task<User?> GetUserWithAddressAsync(int userId);
        Task<User?> GetUserWithOrdersAsync(int userId);
        Task<User?> GetUserWithCartAsync(int userId);
    }
} 