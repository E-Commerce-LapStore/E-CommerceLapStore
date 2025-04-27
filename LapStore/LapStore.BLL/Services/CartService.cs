using LapStore.BLL.Interfaces;
using LapStore.DAL.Data.Entities;
using LapStore.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LapStore.BLL.Services
{
    /// <summary>
    /// Service for managing shopping cart operations
    /// </summary>
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartService> _logger;

        public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a cart for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>The user's cart with included items</returns>
        /// <exception cref="ArgumentException">Thrown when userId is less than or equal to 0</exception>
        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid userId provided: {UserId}", userId);
                throw new ArgumentException("User ID must be greater than 0", nameof(userId));
            }

            try
            {
                var cartRepository = _unitOfWork.GenericRepository<Cart>();
                var query = await cartRepository.GetAllAsync();
                return await query
                    .Where(c => c.UserId == userId)
                    .AsQueryable()
                    .Include(c => c.cartItems)
                        .ThenInclude(ci => ci.product)
                            .ThenInclude(p => p.category)
                    .Include(c => c.cartItems)
                        .ThenInclude(ci => ci.product)
                            .ThenInclude(p => p.productImages)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific cart item
        /// </summary>
        /// <param name="cartId">The ID of the cart</param>
        /// <param name="productId">The ID of the product</param>
        /// <returns>The cart item with included product details</returns>
        /// <exception cref="ArgumentException">Thrown when cartId or productId is less than or equal to 0</exception>
        public async Task<CartItem> GetCartItemAsync(int cartId, int productId)
        {
            if (cartId <= 0 || productId <= 0)
            {
                _logger.LogWarning("Invalid cartId or productId provided: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                throw new ArgumentException("Cart ID and Product ID must be greater than 0");
            }

            try
            {
                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var query = await cartItemRepository.GetAllAsync();
                return await query
                    .Where(ci => ci.CartId == cartId && ci.ProductId == productId)
                    .AsQueryable()
                    .Include(ci => ci.product)
                        .ThenInclude(p => p.category)
                    .Include(ci => ci.product)
                        .ThenInclude(p => p.productImages)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart item: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all items in a cart
        /// </summary>
        /// <param name="cartId">The ID of the cart</param>
        /// <returns>A list of cart items with included product details</returns>
        /// <exception cref="ArgumentException">Thrown when cartId is less than or equal to 0</exception>
        public async Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(int cartId)
        {
            if (cartId <= 0)
            {
                _logger.LogWarning("Invalid cartId provided: {CartId}", cartId);
                throw new ArgumentException("Cart ID must be greater than 0", nameof(cartId));
            }

            try
            {
                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var query = await cartItemRepository.GetAllAsync();
                return await query
                    .Where(ci => ci.CartId == cartId)
                    .AsQueryable()
                    .Include(ci => ci.product)
                        .ThenInclude(p => p.category)
                    .Include(ci => ci.product)
                        .ThenInclude(p => p.productImages)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart items for cart {CartId}", cartId);
                throw;
            }
        }

        /// <summary>
        /// Adds an item to the cart or updates its quantity if it already exists
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="productId">The ID of the product</param>
        /// <param name="quantity">The quantity to add</param>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
        /// <exception cref="InvalidOperationException">Thrown when product is not found</exception>
        public async Task AddItemToCartAsync(int userId, int productId, int quantity)
        {
            if (userId <= 0 || productId <= 0)
            {
                _logger.LogWarning("Invalid userId or productId provided: UserId={UserId}, ProductId={ProductId}", userId, productId);
                throw new ArgumentException("User ID and Product ID must be greater than 0");
            }

            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid quantity provided: {Quantity}", quantity);
                throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get or create cart for user
                var cartRepository = _unitOfWork.GenericRepository<Cart>();
                var query = await cartRepository.GetAllAsync();
                var cart = await query
                    .Where(c => c.UserId == userId)
                    .AsQueryable()
                    .FirstOrDefaultAsync();

                if (cart == null)
                {
                    cart = new Cart { UserId = userId };
                    await cartRepository.AddAsync(cart);
                    await _unitOfWork.CompleteAsync();
                }

                // Get product to check price
                var productRepository = _unitOfWork.GenericRepository<Product>();
                var product = await productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product not found: {ProductId}", productId);
                    throw new InvalidOperationException($"Product with ID {productId} not found");
                }

                // Add or update cart item
                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var cartItemQuery = await cartItemRepository.GetAllAsync();
                var existingItem = await cartItemQuery
                    .Where(ci => ci.CartId == cart.Id && ci.ProductId == productId)
                    .AsQueryable()
                    .FirstOrDefaultAsync();

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    cartItemRepository.Update(existingItem);
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.Price
                    };
                    await cartItemRepository.AddAsync(newItem);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error adding item to cart: UserId={UserId}, ProductId={ProductId}, Quantity={Quantity}", 
                    userId, productId, quantity);
                throw;
            }
        }

        /// <summary>
        /// Updates the quantity of a cart item
        /// </summary>
        /// <param name="cartId">The ID of the cart</param>
        /// <param name="productId">The ID of the product</param>
        /// <param name="quantity">The new quantity</param>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
        /// <exception cref="InvalidOperationException">Thrown when cart item is not found</exception>
        public async Task UpdateCartItemQuantityAsync(int cartId, int productId, int quantity)
        {
            if (cartId <= 0 || productId <= 0)
            {
                _logger.LogWarning("Invalid cartId or productId provided: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                throw new ArgumentException("Cart ID and Product ID must be greater than 0");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var query = await cartItemRepository.GetAllAsync();
                var cartItem = await query
                    .Where(ci => ci.CartId == cartId && ci.ProductId == productId)
                    .AsQueryable()
                    .FirstOrDefaultAsync();

                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item not found: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                    throw new InvalidOperationException($"Cart item not found for cart {cartId} and product {productId}");
                }

                if (quantity <= 0)
                {
                    cartItemRepository.Delete(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    cartItemRepository.Update(cartItem);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating cart item quantity: CartId={CartId}, ProductId={ProductId}, Quantity={Quantity}", 
                    cartId, productId, quantity);
                throw;
            }
        }

        /// <summary>
        /// Removes an item from the cart
        /// </summary>
        /// <param name="cartId">The ID of the cart</param>
        /// <param name="productId">The ID of the product</param>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
        /// <exception cref="InvalidOperationException">Thrown when cart item is not found</exception>
        public async Task RemoveItemFromCartAsync(int cartId, int productId)
        {
            if (cartId <= 0 || productId <= 0)
            {
                _logger.LogWarning("Invalid cartId or productId provided: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                throw new ArgumentException("Cart ID and Product ID must be greater than 0");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var query = await cartItemRepository.GetAllAsync();
                var cartItem = await query
                    .Where(ci => ci.CartId == cartId && ci.ProductId == productId)
                    .AsQueryable()
                    .FirstOrDefaultAsync();

                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item not found: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                    throw new InvalidOperationException($"Cart item not found for cart {cartId} and product {productId}");
                }

                cartItemRepository.Delete(cartItem);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error removing item from cart: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                throw;
            }
        }

        /// <summary>
        /// Removes all items from the cart
        /// </summary>
        /// <param name="cartId">The ID of the cart</param>
        /// <exception cref="ArgumentException">Thrown when cartId is less than or equal to 0</exception>
        public async Task ClearCartAsync(int cartId)
        {
            if (cartId <= 0)
            {
                _logger.LogWarning("Invalid cartId provided: {CartId}", cartId);
                throw new ArgumentException("Cart ID must be greater than 0", nameof(cartId));
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var query = await cartItemRepository.GetAllAsync();
                var cartItems = await query
                    .Where(ci => ci.CartId == cartId)
                    .AsQueryable()
                    .ToListAsync();

                foreach (var item in cartItems)
                {
                    cartItemRepository.Delete(item);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error clearing cart: CartId={CartId}", cartId);
                throw;
            }
        }

        /// <summary>
        /// Calculates the total price of all items in the cart
        /// </summary>
        /// <param name="cartId">The ID of the cart</param>
        /// <returns>The total price of all items in the cart</returns>
        /// <exception cref="ArgumentException">Thrown when cartId is less than or equal to 0</exception>
        public async Task<decimal> GetCartTotalAsync(int cartId)
        {
            if (cartId <= 0)
            {
                _logger.LogWarning("Invalid cartId provided: {CartId}", cartId);
                throw new ArgumentException("Cart ID must be greater than 0", nameof(cartId));
            }

            try
            {
                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var query = await cartItemRepository.GetAllAsync();
                return await query
                    .AsQueryable()
                    .Where(ci => ci.CartId == cartId)
                    .SumAsync(ci => ci.Quantity * ci.UnitPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating cart total: CartId={CartId}", cartId);
                throw;
            }
        }

        /// <summary>
        /// Gets the total number of items in the cart
        /// </summary>
        /// <param name="cartId">The ID of the cart</param>
        /// <returns>The total number of items in the cart</returns>
        /// <exception cref="ArgumentException">Thrown when cartId is less than or equal to 0</exception>
        public async Task<int> GetCartItemCountAsync(int cartId)
        {
            if (cartId <= 0)
            {
                _logger.LogWarning("Invalid cartId provided: {CartId}", cartId);
                throw new ArgumentException("Cart ID must be greater than 0", nameof(cartId));
            }

            try
            {
                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var query = await cartItemRepository.GetAllAsync();
                return await query
                    .AsQueryable()
                    .Where(ci => ci.CartId == cartId)
                    .SumAsync(ci => ci.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item count: CartId={CartId}", cartId);
                throw;
            }
        }

        /// <summary>
        /// Checks if a specific item exists in the cart
        /// </summary>
        /// <param name="cartId">The ID of the cart</param>
        /// <param name="productId">The ID of the product</param>
        /// <returns>True if the item exists in the cart, false otherwise</returns>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
        public async Task<bool> CartItemExistsAsync(int cartId, int productId)
        {
            if (cartId <= 0 || productId <= 0)
            {
                _logger.LogWarning("Invalid cartId or productId provided: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                throw new ArgumentException("Cart ID and Product ID must be greater than 0");
            }

            try
            {
                var cartItemRepository = _unitOfWork.GenericRepository<CartItem>();
                var query = await cartItemRepository.GetAllAsync();
                return await query
                    .AsQueryable()
                    .AnyAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if cart item exists: CartId={CartId}, ProductId={ProductId}", cartId, productId);
                throw;
            }
        }
    }
} 