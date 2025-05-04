using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUserNameAsync(string userName);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserWithAddressAsync(int userId);
        Task<User?> GetUserWithOrdersAsync(int userId);
        Task<User?> GetUserWithCartAsync(int userId);
        Task<bool> CreateUserAsync(User user);
        Task<bool> CreateUserWithAddressAsync(User user, Address address);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> IsEmailExistAsync(string email, int? userId = null);
        Task<bool> IsUserNameExistAsync(string userName, int? userId = null);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole);
        Task<bool> UpdateUserAddressAsync(int userId, int addressId);
        Task<User?> AuthenticateAsync(string userName, string password);
    }
} 