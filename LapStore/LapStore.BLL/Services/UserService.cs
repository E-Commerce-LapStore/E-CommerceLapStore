using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.DAL;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using LapStore.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LapStore.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _userRepository.GetUserByUserNameAsync(userName);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserWithAddressAsync(int userId)
        {
            return await _userRepository.GetUserWithAddressAsync(userId);
        }

        public async Task<User?> GetUserWithOrdersAsync(int userId)
        {
            return await _userRepository.GetUserWithOrdersAsync(userId);
        }

        public async Task<User?> GetUserWithCartAsync(int userId)
        {
            return await _userRepository.GetUserWithCartAsync(userId);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
                await _userRepository.AddAsync(user);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                //_loggingService.LogError("Error creating user: {UserName}", ex, user.UserName);
                return false;
            }
        }

        public async Task<bool> CreateUserWithAddressAsync(User user, Address address)
        {
            // Validate Address
            if (string.IsNullOrWhiteSpace(address.Street) ||
                string.IsNullOrWhiteSpace(address.City) ||
                string.IsNullOrWhiteSpace(address.Country) ||
                string.IsNullOrWhiteSpace(address.Governorate) ||
                string.IsNullOrWhiteSpace(address.ZipCode))
            {
                throw new ArgumentException("Address fields must not be null or empty.");
            }

            // Validate User
            if (string.IsNullOrWhiteSpace(user.UserName) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName))
            {
                throw new ArgumentException("User fields must not be null or empty.");
            }

            var strategy = _unitOfWork.Context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var addressRepository = _unitOfWork.GenericRepository<Address>();
                    await addressRepository.AddAsync(address);
                    await _unitOfWork.CompleteAsync();

                    user.AddressId = address.Id;
                    user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
                    await _userRepository.AddAsync(user);
                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    return true;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(user.Id);
                if (existingUser == null)
                    return false;

                // Preserve the existing password hash
                user.PasswordHash = existingUser.PasswordHash;
                
                // Update other user properties
                existingUser.UserName = user.UserName;
                existingUser.Role = user.Role;
                existingUser.Gender = user.Gender;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.BirthDate = user.BirthDate;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.AddressId = user.AddressId;

                await _userRepository.UpdateAsync(existingUser);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return false;
                _userRepository.Delete(user);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                //_loggingService.LogError("Error deleting user: {UserId}", ex, id);
                return false;
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
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return false;

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
                if (result == PasswordVerificationResult.Failed) return false;

                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
               // _loggingService.LogError("Error changing password for user: {UserId}", ex, userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return false;

                user.Role = newRole;
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                //_loggingService.LogError("Error updating role for user: {UserId}", ex, userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserAddressAsync(int userId, int addressId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return false;

                user.AddressId = addressId;
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                //_loggingService.LogError("Error updating address for user: {UserId}", ex, userId);
                return false;
            }
        }

        public async Task<User?> AuthenticateAsync(string userName, string password)
        {
            try
            {
                var user = await _userRepository.GetUserByUserNameAsync(userName);
                if (user == null) return null;

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (result == PasswordVerificationResult.Failed) return null;

                return user;
            }
            catch (Exception ex)
            {
                //_loggingService.LogError("Error authenticating user: {UserName}", ex, userName);
                return null;
            }
        }
    }
}