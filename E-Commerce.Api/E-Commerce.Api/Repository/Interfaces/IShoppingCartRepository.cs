using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<Cart?> GetActiveCartByCustomerIdAsync(int customerId);
        Task<Cart?> GetCartByIdAsync(int cartId);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<CartItem?> GetCartItemByCartIdAndProductIdAsync(int cartId, int productId);
        Task CreateCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task CreateCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(CartItem cartItem);
        Task DeleteCartItemsRangeAsync(List<CartItem> cartItems);
    }
}

