using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task<CartItem> GetCartItemAsync(int cartId, int productId);
        Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(int cartId);
        Task AddItemToCartAsync(int userId, int productId, int quantity);
        Task UpdateCartItemQuantityAsync(int cartId, int productId, int quantity);
        Task RemoveItemFromCartAsync(int cartId, int productId);
        Task ClearCartAsync(int cartId);
        Task<decimal> GetCartTotalAsync(int cartId);
        Task<int> GetCartItemCountAsync(int cartId);
        Task<bool> CartItemExistsAsync(int cartId, int productId);
    }
} 