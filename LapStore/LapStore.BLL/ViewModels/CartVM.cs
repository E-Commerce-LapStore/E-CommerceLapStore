using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.ViewModels
{
    public class CartVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ICollection<CartItemVM> CartItems { get; set; } = new List<CartItemVM>();

        public decimal Subtotal => CartItems?.Sum(item => item.TotalPrice) ?? 0;
        public decimal ShippingCost => 0; // Free shipping for now
        public decimal Total => Subtotal + ShippingCost;

        public static CartVM FromCart(Cart cart)
        {
            var cartVM = new CartVM
            {
                Id = cart.Id,
                UserId = cart.UserId
            };

            if (cart.cartItems != null)
            {
                cartVM.CartItems = cart.cartItems.Select(item => new CartItemVM
                {
                    CartId = item.CartId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ProductName = item.product?.Name ?? "",
                    ProductCategory = item.product?.category?.Name ?? "",
                    MainImageUrl = item.product?.productImages?.FirstOrDefault(i => i.IsMain)?.URL ?? ""
                }).ToList();
            }

            return cartVM;
        }

        public static Cart FromCartVM(CartVM cartVM)
        {
            return new Cart
            {
                Id = cartVM.Id,
                UserId = cartVM.UserId
            };
        }
    }
} 