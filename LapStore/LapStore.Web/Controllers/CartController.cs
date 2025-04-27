using Microsoft.AspNetCore.Mvc;
using LapStore.BLL.Interfaces;
using LapStore.BLL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace LapStore.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId <= 0)
                {
                    _logger.LogWarning("Invalid user ID when accessing cart");
                    return RedirectToAction("Login", "Account");
                }

                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    _logger.LogInformation("No cart found for user {UserId}, creating empty cart view model", userId);
                    return View(new CartVM { UserId = userId });
                }

                var cartVM = CartVM.FromCart(cart);
                return View(cartVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving cart");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int productId, int quantity)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId <= 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (quantity <= 0)
                {
                    return BadRequest(new { success = false, message = "Quantity must be greater than 0" });
                }

                await _cartService.AddItemToCartAsync(userId, productId, quantity);
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while adding item to cart");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding item to cart");
                return StatusCode(500, new { success = false, message = "An error occurred while adding the item to cart" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartId, int productId, int quantity)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId <= 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || cart.Id != cartId)
                {
                    return Forbid();
                }

                await _cartService.UpdateCartItemQuantityAsync(cartId, productId, quantity);
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating cart item quantity");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart item quantity");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the quantity" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int cartId, int productId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId <= 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || cart.Id != cartId)
                {
                    return Forbid();
                }

                await _cartService.RemoveItemFromCartAsync(cartId, productId);
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while removing item from cart");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing item from cart");
                return StatusCode(500, new { success = false, message = "An error occurred while removing the item" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear(int cartId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId <= 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || cart.Id != cartId)
                {
                    return Forbid();
                }

                await _cartService.ClearCartAsync(cartId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing cart");
                return StatusCode(500, new { success = false, message = "An error occurred while clearing the cart" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemCount(int cartId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId <= 0)
                {
                    return Unauthorized();
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || cart.Id != cartId)
                {
                    return Forbid();
                }

                var count = await _cartService.GetCartItemCountAsync(cartId);
                return Json(new { success = true, count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cart item count");
                return StatusCode(500, new { success = false, message = "An error occurred while getting the cart count" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartTotal(int cartId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId <= 0)
                {
                    return Unauthorized();
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || cart.Id != cartId)
                {
                    return Forbid();
                }

                var total = await _cartService.GetCartTotalAsync(cartId);
                return Json(new { success = true, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cart total");
                return StatusCode(500, new { success = false, message = "An error occurred while getting the cart total" });
            }
        }
    }
} 