using LapStore.BLL.Interfaces;
using LapStore.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using LapStore.BLL.Services.Interfaces;
using LapStore.DAL;

namespace LapStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILoggingService _loggingService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUserService userService, ILoggingService loggingService, UserManager<User> userManager, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _loggingService = loggingService;
            _userManager = userManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.AuthenticateAsync(model.UserName, model.Password);
                    if (user != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Role, user.Role.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            AllowRefresh = true
                        };

                        await HttpContext.SignInAsync(
                            IdentityConstants.ApplicationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", "Invalid username or password");
                }
                catch (Exception ex)
                {
                    _loggingService.LogError("Error during login for user: {UserName}", ex, model.UserName);
                    ModelState.AddModelError("", "An error occurred during login. Please try again.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Check if username exists
                var existingUserByName = await _userManager.FindByNameAsync(model.UserName);
                if (existingUserByName != null)
                {
                    ModelState.AddModelError("UserName", "Username is already taken.");
                    return View(model);
                }

                // Check if email exists
                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                try
                {
                    // Create the address first
                    var address = new Address
                    {
                        Street = model.Street,
                        City = model.City,
                        Governorate = model.Governorate,
                        ZipCode = model.ZipCode,
                        Country = model.Country
                    };

                    // Save the address using the UnitOfWork's generic repository
                    var addressRepo = _unitOfWork.GenericRepository<Address>();
                    await addressRepo.AddAsync(address);
                    await _unitOfWork.CompleteAsync();

                    // Create the user with all required information
                    var user = new User
                    {
                        Email = model.Email,
                        UserName = model.UserName,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        BirthDate = model.BirthDate,
                        Gender = model.Gender,
                        Role = UserRole.Customer, // Default role for new users
                        AddressId = address.Id // Set the address ID after it's saved
                    };

                    // Use ASP.NET Identity to create the user (handles password hashing, validation, etc.)
                    var identityResult = await _userManager.CreateAsync(user, model.Password);
                    if (identityResult.Succeeded)
                    {
                        // Send confirmation email
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, token = token }, Request.Scheme);
                        await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);

                        TempData["Success"] = "Registration successful! Please check your email to confirm your account.";
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        // If user creation fails, we should clean up the address
                        addressRepo.Delete(address);
                        await _unitOfWork.CompleteAsync();

                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    _loggingService.LogError("Error during registration for user: {UserName}", ex, model.UserName);
                    ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                
                var userVM = UserVM.FromUser(user);
                return View(userVM);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error retrieving user profile", ex);
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var userVM = UserVM.FromUser(user);
                return View(userVM);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error retrieving user profile for editing", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    if (userId != userVM.Id)
                    {
                        return Unauthorized();
                    }

                    var result = await _userService.UpdateUserAsync(UserVM.FromUserVM(userVM));
                    if (result)
                    {
                        TempData["Success"] = "Profile updated successfully";
                        return RedirectToAction(nameof(Profile));
                    }

                    ModelState.AddModelError("", "Failed to update profile");
                }
                catch (Exception ex)
                {
                    _loggingService.LogError("Error updating user profile: {UserId}", ex, userVM.Id);
                    ModelState.AddModelError("", "An error occurred while updating your profile.");
                }
            }

            return View(userVM);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return View(new ChangePasswordVM { UserId = userId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    if (userId != model.UserId)
                    {
                        return Unauthorized();
                    }

                    var result = await _userService.ChangePasswordAsync(
                        model.UserId,
                        model.CurrentPassword,
                        model.NewPassword
                    );

                    if (result)
                    {
                        TempData["Success"] = "Password changed successfully";
                        return RedirectToAction(nameof(Profile));
                    }

                    ModelState.AddModelError("", "Failed to change password. Please check your current password.");
                }
                catch (Exception ex)
                {
                    _loggingService.LogError("Error changing password for user: {UserId}", ex, model.UserId);
                    ModelState.AddModelError("", "An error occurred while changing your password.");
                }
            }

            return View(model);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsEmailExist(string email, int? id)
        {
            var result = await _userService.IsEmailExistAsync(email, id);
            return Json(!result);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsUserNameExist(string userName, int? id)
        {
            var result = await _userService.IsUserNameExistAsync(userName, id);
            return Json(!result);
        }
    }
} 