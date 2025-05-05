using LapStore.BLL.DTOs.AccountDTO;
using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace LapStore.BLL.Interfaces
{
    public interface IUserService
    {
        // Authentication methods
        Task<string> Register(RegisterDTO registerDTO);
        Task<string> Login(LoginDTO loginDTO);
        Task<bool> LogoutAsync(string username);

        // User profile methods
        Task<UserInfoDTO> GetUserProfileAsync(int userId);
        Task<bool> UpdateUserProfileAsync(int userId, UpdateProfileDTO updateProfileDTO);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO);

        // Address methods
        Task<AddressInfoDTO> GetUserAddressAsync(int userId);
        Task<bool> AddAddressAsync(int userId, AddAddressDTO addressDTO);
        Task<bool> UpdateAddressAsync(int userId, int addressId, UpdateAddressDTO addressDTO);
    }
}