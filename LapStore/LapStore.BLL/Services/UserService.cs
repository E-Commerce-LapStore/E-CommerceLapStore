using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.DAL;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using LapStore.BLL.Interfaces;

namespace LapStore.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILoggingService _loggingService;

        public UserService(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            ILoggingService loggingService)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _loggingService = loggingService;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to get user by ID: {UserId}", id);
                var user = await _userRepository.GetByIdAsync(id);
                
                if (user == null)
                {
                    _loggingService.LogWarning("User not found with ID: {UserId}", id);
                    return null;
                }

                _loggingService.LogInformation("Successfully retrieved user: {UserId}", id);
                return user;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user by ID: {UserId}", ex, id);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("GetUserByIdAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to get user by email: {Email}", email);
                var user = await _userRepository.GetUserByEmailAsync(email);
                
                if (user == null)
                {
                    _loggingService.LogWarning("User not found with email: {Email}", email);
                    return null;
                }

                _loggingService.LogInformation("Successfully retrieved user by email: {Email}", email);
                return user;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user by email: {Email}", ex, email);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("GetUserByEmailAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to get user by username: {UserName}", userName);
                var user = await _userRepository.GetUserByUserNameAsync(userName);
                
                if (user == null)
                {
                    _loggingService.LogWarning("User not found with username: {UserName}", userName);
                    return null;
                }

                _loggingService.LogInformation("Successfully retrieved user by username: {UserName}", userName);
                return user;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user by username: {UserName}", ex, userName);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("GetUserByUserNameAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to get all users");
                var users = await _userRepository.GetAllAsync();
                
                _loggingService.LogInformation("Successfully retrieved {Count} users", users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting all users", ex);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("GetAllUsersAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<User?> GetUserWithAddressAsync(int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to get user with address: {UserId}", userId);
                var user = await _userRepository.GetUserWithAddressAsync(userId);
                
                if (user == null)
                {
                    _loggingService.LogWarning("User not found with address: {UserId}", userId);
                    return null;
                }

                _loggingService.LogInformation("Successfully retrieved user with address: {UserId}", userId);
                return user;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user with address: {UserId}", ex, userId);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("GetUserWithAddressAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<User?> GetUserWithOrdersAsync(int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to get user with orders: {UserId}", userId);
                var user = await _userRepository.GetUserWithOrdersAsync(userId);
                
                if (user == null)
                {
                    _loggingService.LogWarning("User not found with orders: {UserId}", userId);
                    return null;
                }

                _loggingService.LogInformation("Successfully retrieved user with orders: {UserId}", userId);
                return user;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user with orders: {UserId}", ex, userId);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("GetUserWithOrdersAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<User?> GetUserWithCartAsync(int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to get user with cart: {UserId}", userId);
                var user = await _userRepository.GetUserWithCartAsync(userId);
                
                if (user == null)
                {
                    _loggingService.LogWarning("User not found with cart: {UserId}", userId);
                    return null;
                }

                _loggingService.LogInformation("Successfully retrieved user with cart: {UserId}", userId);
                return user;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user with cart: {UserId}", ex, userId);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("GetUserWithCartAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to create new user: {UserName}", user.UserName);
                
                if (await _userRepository.IsEmailExistAsync(user.Email))
                {
                    _loggingService.LogWarning("Email already exists: {Email}", user.Email);
                    return false;
                }

                if (await _userRepository.IsUserNameExistAsync(user.UserName))
                {
                    _loggingService.LogWarning("Username already exists: {UserName}", user.UserName);
                    return false;
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
                
                await _userRepository.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                _loggingService.LogInformation("Successfully created new user: {UserName}", user.UserName);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while creating user: {UserName}", ex, user.UserName);
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("CreateUserAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to update user: {UserId}", user.Id);
                
                var existingUser = await _userRepository.GetByIdAsync(user.Id);
                if (existingUser == null)
                {
                    _loggingService.LogWarning("User not found for update: {UserId}", user.Id);
                    return false;
                }

                if (await _userRepository.IsEmailExistAsync(user.Email, user.Id))
                {
                    _loggingService.LogWarning("Email already exists for another user: {Email}", user.Email);
                    return false;
                }

                if (await _userRepository.IsUserNameExistAsync(user.UserName, user.Id))
                {
                    _loggingService.LogWarning("Username already exists for another user: {UserName}", user.UserName);
                    return false;
                }

                user.PasswordHash = existingUser.PasswordHash; // Preserve existing password

                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync();

                _loggingService.LogInformation("Successfully updated user: {UserId}", user.Id);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while updating user: {UserId}", ex, user.Id);
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("UpdateUserAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to delete user: {UserId}", id);
                
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _loggingService.LogWarning("User not found for deletion: {UserId}", id);
                    return false;
                }

                _userRepository.Delete(user);
                await _unitOfWork.CompleteAsync();

                _loggingService.LogInformation("Successfully deleted user: {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while deleting user: {UserId}", ex, id);
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("DeleteUserAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<bool> IsEmailExistAsync(string email, int? userId = null)
        {
            return await _userRepository.IsEmailExistAsync(email, userId);
        }

        public async Task<bool> IsUserNameExistAsync(string userName, int? userId = null)
        {
            return await _userRepository.IsUserNameExistAsync(userName, userId);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to change password for user: {UserId}", userId);
                
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _loggingService.LogWarning("User not found for password change: {UserId}", userId);
                    return false;
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
                if (result == PasswordVerificationResult.Failed)
                {
                    _loggingService.LogWarning("Invalid current password for user: {UserId}", userId);
                    return false;
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync();

                _loggingService.LogInformation("Successfully changed password for user: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while changing password for user: {UserId}", ex, userId);
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("ChangePasswordAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to update role for user: {UserId}", userId);
                
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _loggingService.LogWarning("User not found for role update: {UserId}", userId);
                    return false;
                }

                user.Role = newRole;
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync();

                _loggingService.LogInformation("Successfully updated role for user: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while updating role for user: {UserId}", ex, userId);
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("UpdateUserRoleAsync", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<bool> UpdateUserAddressAsync(int userId, int addressId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _loggingService.LogInformation("Attempting to update address for user: {UserId}", userId);
                
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _loggingService.LogWarning("User not found for address update: {UserId}", userId);
                    return false;
                }

                user.AddressId = addressId;
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync();

                _loggingService.LogInformation("Successfully updated address for user: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while updating address for user: {UserId}", ex, userId);
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _loggingService.LogPerformance("UpdateUserAddressAsync", stopwatch.ElapsedMilliseconds);
            }
        }
    }
} 