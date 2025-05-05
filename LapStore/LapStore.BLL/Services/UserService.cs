using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.DAL;
using Microsoft.AspNetCore.Identity;
using LapStore.BLL.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using LapStore.BLL.DTOs.AccountDTO;

namespace LapStore.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Address> _genericRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        
        public UserService(
            IUnitOfWork unitOfWork,
            IGenericRepository<Address> genericRepository,
            IUserRepository userRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string> Register(RegisterDTO registerDTO)
        {
            var user = RegisterDTO.FromRegisterDTO(registerDTO);
            
            // Set default role to Customer for new registrations
            user.Role = UserRole.Customer;
            
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                // Create token with appropriate claims
                List<Claim> claims = new List<Claim>();
                
                claims.Add(new Claim("Role", user.Role.ToString()));
                claims.Add(new Claim("UserName", registerDTO.UserName));
                claims.Add(new Claim("Email", registerDTO.Email));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                await _userManager.AddClaimsAsync(user, claims);

                return GenerateToken(claims);
            }
            else
            {
                // Log the errors for debugging
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"Registration failed: {errors}");
                return null;
            }
        }

        public async Task<string> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.UserName);
            if (user == null)
            {
                return null;
            }

            var check = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!check) // Fixed boolean check
            {
                return null;
            }

            var claims = await _userManager.GetClaimsAsync(user);
            return GenerateToken(claims);
        }
        public async Task<bool> LogoutAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return false;
                }

                // Sign out the user using SignInManager
                await _signInManager.SignOutAsync();

                // Note: JWT tokens cannot be invalidated on the server-side once issued
                // The client should discard the token

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get user profile information
        public async Task<UserInfoDTO> GetUserProfileAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;

            return UserInfoDTO.FromUser(user);
        }

        // Update user profile
        public async Task<bool> UpdateUserProfileAsync(int userId, UpdateProfileDTO updateProfileDTO)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            // Check if username exists (except for the current user)
            if (!string.IsNullOrEmpty(updateProfileDTO.UserName) && user.UserName != updateProfileDTO.UserName)
            {
                var existingUserWithUsername = await _userManager.FindByNameAsync(updateProfileDTO.UserName);
                if (existingUserWithUsername != null)
                    return false;
                
                user.UserName = updateProfileDTO.UserName;
            }

            // Check if email exists (except for the current user)
            if (!string.IsNullOrEmpty(updateProfileDTO.Email) && user.Email != updateProfileDTO.Email)
            {
                var existingUserWithEmail = await _userManager.FindByEmailAsync(updateProfileDTO.Email);
                if (existingUserWithEmail != null)
                    return false;
                
                user.Email = updateProfileDTO.Email;
            }

            // Update other properties
            user = UpdateProfileDTO.FromUpdateProfileDTO(updateProfileDTO);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        // Change password
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(
                user, 
                changePasswordDTO.CurrentPassword, 
                changePasswordDTO.NewPassword
            );
            
            return result.Succeeded;
        }

        // Get user address
        public async Task<AddressInfoDTO> GetUserAddressAsync(int userId)
        {
            var user = await _userRepository.GetUserWithAddressAsync(userId);
            if (user == null || user.address == null)
                return null;

            return AddressInfoDTO.FromAddress(user.address);
        }

        // Add address
        public async Task<bool> AddAddressAsync(int userId, AddAddressDTO addressDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return false;

                // Create new address
                var address = AddAddressDTO.FromAddressDTO(addressDTO);

                // Add address to repository
                await _genericRepository.AddAsync(address);
                await _unitOfWork.CompleteAsync();

                // Update user with new address ID
                user.AddressId = address.Id;
                var result = await _userManager.UpdateAsync(user);
                
                return result.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Update address
        public async Task<bool> UpdateAddressAsync(int userId, int addressId, UpdateAddressDTO addressDTO)
        {
            try
            {
                var user = await _userRepository.GetUserWithAddressAsync(userId);
                if (user == null || user.address == null || user.address.Id != addressId)
                    return false;

                var address = user.address;

                // Update address properties
                address = UpdateAddressDTO.FromAddressDTO(addressDTO);

                // Save changes
                 _genericRepository.Update(address);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GenerateToken(IList<Claim> claims)
        {
            var securitykeystring = _configuration.GetSection("SecretKey").Value;
            var securtykeyByte = Encoding.ASCII.GetBytes(securitykeystring);
            SecurityKey securityKey = new SymmetricSecurityKey(securtykeyByte);
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expire = DateTime.UtcNow.AddDays(2);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                claims: claims, 
                expires: expire, 
                signingCredentials: signingCredentials
            );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string token = handler.WriteToken(jwtSecurityToken);

            return token;
        }
    }
}