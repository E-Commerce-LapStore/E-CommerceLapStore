using System.ComponentModel.DataAnnotations;
using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.ViewModels
{
    public class CartItemVM
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        
        // Product details
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public string MainImageUrl { get; set; }
        
        public decimal TotalPrice => Quantity * UnitPrice;

        public static CartItemVM FromCartItem(CartItem cartItem)
        {
            if (cartItem == null)
                return null;

            return new CartItemVM
            {
                CartId = cartItem.CartId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                ProductName = cartItem.product?.Name ?? "",
                ProductCategory = cartItem.product?.category?.Name ?? "",
                MainImageUrl = cartItem.product?.productImages?.FirstOrDefault(i => i.IsMain)?.URL ?? ""
            };
        }

        public static CartItem FromCartItemVM(CartItemVM cartItemVM)
        {
            if (cartItemVM == null)
                return null;

            return new CartItem
            {
                CartId = cartItemVM.CartId,
                ProductId = cartItemVM.ProductId,
                Quantity = cartItemVM.Quantity,
                UnitPrice = cartItemVM.UnitPrice
            };
        }
    }
} 