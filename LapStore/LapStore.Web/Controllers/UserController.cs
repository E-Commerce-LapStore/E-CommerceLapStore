using LapStore.BLL.Interfaces;
using LapStore.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LapStore.BLL.Services;

namespace LapStore.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILoggingService _loggingService;

        public UserController(IUserService userService, ILoggingService loggingService)
        {
            _userService = userService;
            _loggingService = loggingService;
        }

        // GET: User
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var userVMs = users.Select(UserVM.FromUser).ToList();
                return View(userVMs);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting all users", ex);
                return View("Error");
            }
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = await _userService.GetUserWithAddressAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(UserVM.FromUser(user));
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user details: {UserId}", ex, id);
                return View("Error");
            }
        }

        // GET: User/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new UserVM());
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = UserVM.FromUserVM(userVM);
                    var result = await _userService.CreateUserAsync(user);
                    
                    if (result)
                    {
                        TempData["Success"] = "User created successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", "Failed to create user. Please try again.");
                }
                catch (Exception ex)
                {
                    _loggingService.LogError("Error occurred while creating user", ex);
                    ModelState.AddModelError("", "An error occurred while creating the user.");
                }
            }
            return View(userVM);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                var userVM = UserVM.FromUser(user);
                return View(userVM);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user for edit: {UserId}", ex, id);
                return View("Error");
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserVM userVM)
        {
            if (id != userVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = UserVM.FromUserVM(userVM);
                    var result = await _userService.UpdateUserAsync(user);
                    
                    if (result)
                    {
                        TempData["Success"] = "User updated successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", "Failed to update user. Please try again.");
                }
                catch (Exception ex)
                {
                    _loggingService.LogError("Error occurred while updating user: {UserId}", ex, id);
                    ModelState.AddModelError("", "An error occurred while updating the user.");
                }
            }
            return View(userVM);
        }

        // GET: User/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(UserVM.FromUser(user));
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user for delete: {UserId}", ex, id);
                return View("Error");
            }
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                TempData["Success"] = "User deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while deleting user: {UserId}", ex, id);
                return View("Error");
            }
        }

        // GET: User/ChangePassword/5
        public async Task<IActionResult> ChangePassword(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(new ChangePasswordVM { UserId = id });
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error occurred while getting user for password change: {UserId}", ex, id);
                return View("Error");
            }
        }

        // POST: User/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _userService.ChangePasswordAsync(
                        model.UserId,
                        model.CurrentPassword,
                        model.NewPassword
                    );

                    if (result)
                    {
                        TempData["Success"] = "Password changed successfully";
                        return RedirectToAction(nameof(Details), new { id = model.UserId });
                    }

                    ModelState.AddModelError("", "Failed to change password. Please check your current password.");
                }
                catch (Exception ex)
                {
                    _loggingService.LogError("Error occurred while changing password for user: {UserId}", ex, model.UserId);
                    ModelState.AddModelError("", "An error occurred while changing the password.");
                }
            }
            return View(model);
        }

        // Remote validation for email
        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailExist(string email, int? id)
        {
            var result = await _userService.IsEmailExistAsync(email, id);
            return Json(!result);
        }

        // Remote validation for username
        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUserNameExist(string userName, int? id)
        {
            var result = await _userService.IsUserNameExistAsync(userName, id);
            return Json(!result);
        }
    }
} 