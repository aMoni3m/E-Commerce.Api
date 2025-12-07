using AutoMapper;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.ShoppingCartDTOs;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;

namespace E_Commerce.Api.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, IMapper mapper)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CartResponseDTO>> GetCartByCustomerIdAsync(int customerId)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetActiveCartByCustomerIdAsync(customerId);

                if (cart == null)
                {
                    var emptyCartDTO = new CartResponseDTO
                    {
                        CustomerId = customerId,
                        IsCheckedOut = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItemResponseDTO>(),
                        TotalBasePrice = 0,
                        TotalDiscount = 0,
                        TotalAmount = 0
                    };

                    return new ApiResponse<CartResponseDTO>(200, emptyCartDTO);
                }

                var cartDTO = _mapper.Map<CartResponseDTO>(cart);
                return new ApiResponse<CartResponseDTO>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponseDTO>> AddToCartAsync(AddToCartDTO addToCartDTO)
        {
            try
            {
                var product = await _shoppingCartRepository.GetProductByIdAsync(addToCartDTO.ProductId);
                if (product == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Product not found.");
                }

                if (addToCartDTO.Quantity > product.StockQuantity)
                {
                    return new ApiResponse<CartResponseDTO>(400, $"Only {product.StockQuantity} units of {product.Name} are available.");
                }

                var cart = await _shoppingCartRepository.GetActiveCartByCustomerIdAsync(addToCartDTO.CustomerId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        CustomerId = addToCartDTO.CustomerId,
                        IsCheckedOut = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItem>()
                    };

                    await _shoppingCartRepository.CreateCartAsync(cart);
                }

                var existingCartItem = await _shoppingCartRepository.GetCartItemByCartIdAndProductIdAsync(cart.Id, addToCartDTO.ProductId);

                if (existingCartItem != null)
                {
                    if (existingCartItem.Quantity + addToCartDTO.Quantity > product.StockQuantity)
                    {
                        return new ApiResponse<CartResponseDTO>(400, $"Adding {addToCartDTO.Quantity} exceeds available stock.");
                    }

                    existingCartItem.Quantity += addToCartDTO.Quantity;
                    existingCartItem.TotalPrice = (existingCartItem.UnitPrice - existingCartItem.Discount) * existingCartItem.Quantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;

                    await _shoppingCartRepository.UpdateCartItemAsync(existingCartItem);
                }
                else
                {
                    var discount = product.DiscountPercentage > 0 ? product.Price * product.DiscountPercentage / 100 : 0;

                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = product.Id,
                        Quantity = addToCartDTO.Quantity,
                        UnitPrice = product.Price,
                        Discount = discount,
                        TotalPrice = (product.Price - discount) * addToCartDTO.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _shoppingCartRepository.CreateCartItemAsync(cartItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _shoppingCartRepository.UpdateCartAsync(cart);

                cart = await _shoppingCartRepository.GetCartByIdAsync(cart.Id) ?? new Cart();

                var cartDTO = _mapper.Map<CartResponseDTO>(cart);
                return new ApiResponse<CartResponseDTO>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponseDTO>> UpdateCartItemAsync(UpdateCartItemDTO updateCartItemDTO)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetActiveCartByCustomerIdAsync(updateCartItemDTO.CustomerId);
                if (cart == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Active cart not found.");
                }

                var cartItem = await _shoppingCartRepository.GetCartItemByIdAsync(updateCartItemDTO.CartItemId);
                if (cartItem == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Cart item not found.");
                }

                if (cartItem.Product == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Product not found.");
                }

                if (updateCartItemDTO.Quantity > cartItem.Product.StockQuantity)
                {
                    return new ApiResponse<CartResponseDTO>(400, $"Only {cartItem.Product.StockQuantity} units of {cartItem.Product.Name} are available.");
                }

                cartItem.Quantity = updateCartItemDTO.Quantity;
                cartItem.TotalPrice = (cartItem.UnitPrice - cartItem.Discount) * cartItem.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                await _shoppingCartRepository.UpdateCartItemAsync(cartItem);

                cart.UpdatedAt = DateTime.UtcNow;
                await _shoppingCartRepository.UpdateCartAsync(cart);

                cart = await _shoppingCartRepository.GetCartByIdAsync(cart.Id) ?? new Cart();

                var cartDTO = _mapper.Map<CartResponseDTO>(cart);
                return new ApiResponse<CartResponseDTO>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponseDTO>> RemoveCartItemAsync(RemoveCartItemDTO removeCartItemDTO)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetActiveCartByCustomerIdAsync(removeCartItemDTO.CustomerId);
                if (cart == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Active cart not found.");
                }

                var cartItem = await _shoppingCartRepository.GetCartItemByIdAsync(removeCartItemDTO.CartItemId);
                if (cartItem == null)
                {
                    return new ApiResponse<CartResponseDTO>(404, "Cart item not found.");
                }

                await _shoppingCartRepository.DeleteCartItemAsync(cartItem);

                cart.UpdatedAt = DateTime.UtcNow;
                await _shoppingCartRepository.UpdateCartAsync(cart);

                cart = await _shoppingCartRepository.GetCartByIdAsync(cart.Id) ?? new Cart();

                var cartDTO = _mapper.Map<CartResponseDTO>(cart);
                return new ApiResponse<CartResponseDTO>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> ClearCartAsync(int customerId)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetActiveCartByCustomerIdAsync(customerId);
                if (cart == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Active cart not found.");
                }

                if (cart.CartItems != null && cart.CartItems.Any())
                {
                    await _shoppingCartRepository.DeleteCartItemsRangeAsync(cart.CartItems.ToList());

                    cart.UpdatedAt = DateTime.UtcNow;
                    await _shoppingCartRepository.UpdateCartAsync(cart);
                }

                var confirmation = new ConfirmationResponseDTO
                {
                    Message = "Cart has been cleared successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

    }
}
