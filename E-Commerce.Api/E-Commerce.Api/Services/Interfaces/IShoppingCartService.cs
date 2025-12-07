using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.ShoppingCartDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<ApiResponse<CartResponseDTO>> GetCartByCustomerIdAsync(int customerId);
        Task<ApiResponse<CartResponseDTO>> AddToCartAsync(AddToCartDTO addToCartDTO);
        Task<ApiResponse<CartResponseDTO>> UpdateCartItemAsync(UpdateCartItemDTO updateCartItemDTO);
        Task<ApiResponse<CartResponseDTO>> RemoveCartItemAsync(RemoveCartItemDTO removeCartItemDTO);
        Task<ApiResponse<ConfirmationResponseDTO>> ClearCartAsync(int customerId);
    }
}

