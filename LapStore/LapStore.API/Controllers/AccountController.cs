using LapStore.BLL.DTOs.AccountDTO;
using LapStore.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LapStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            var result = await _userService.Login(loginDTO);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            return Ok(new { token = result });
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.Register(registerDTO);
            if (result == null)
            {
                return BadRequest(new { message = "Registration failed. Please check your information and try again." });
            }
            return Ok(new { token = result });
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<ActionResult> Logout()
        {
            var username = User.FindFirst("UserName")?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = await _userService.LogoutAsync(username);
            if (!result)
            {
                return BadRequest(new { message = "Logout failed" });
            }

            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize]
        [HttpGet("Profile")]
        public async Task<ActionResult> GetProfile()
        {
            // Get the user ID from the token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            int userId = int.Parse(userIdClaim.Value);
            var userProfile = await _userService.GetUserProfileAsync(userId);

            if (userProfile == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(userProfile);
        }

        [Authorize]
        [HttpPut("Profile")]
        public async Task<ActionResult> UpdateProfile(UpdateProfileDTO updateProfileDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            int userId = int.Parse(userIdClaim.Value);
            var result = await _userService.UpdateUserProfileAsync(userId, updateProfileDTO);

            if (!result)
            {
                return BadRequest(new { message = "Failed to update profile. The username or email may already be in use." });
            }

            return Ok(new { message = "Profile updated successfully" });
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            int userId = int.Parse(userIdClaim.Value);
            var result = await _userService.ChangePasswordAsync(userId, changePasswordDTO);

            if (!result)
            {
                return BadRequest(new { message = "Failed to change password. Please check your current password." });
            }

            return Ok(new { message = "Password changed successfully" });
        }

        [Authorize]
        [HttpGet("Address")]
        public async Task<ActionResult> GetAddress()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            int userId = int.Parse(userIdClaim.Value);
            var address = await _userService.GetUserAddressAsync(userId);

            if (address == null)
            {
                return NotFound(new { message = "Address not found" });
            }

            return Ok(address);
        }

        [Authorize]
        [HttpPost("Address")]
        public async Task<ActionResult> AddAddress(AddAddressDTO addressDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            int userId = int.Parse(userIdClaim.Value);
            var result = await _userService.AddAddressAsync(userId, addressDTO);

            if (!result)
            {
                return BadRequest(new { message = "Failed to add address" });
            }

            return Ok(new { message = "Address added successfully" });
        }

        [Authorize]
        [HttpPut("Address/{addressId}")]
        public async Task<ActionResult> UpdateAddress(int addressId, UpdateAddressDTO addressDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            int userId = int.Parse(userIdClaim.Value);
            var result = await _userService.UpdateAddressAsync(userId, addressId, addressDTO);

            if (!result)
            {
                return BadRequest(new { message = "Failed to update address. Address not found or doesn't belong to the user." });
            }

            return Ok(new { message = "Address updated successfully" });
        }
    }
}