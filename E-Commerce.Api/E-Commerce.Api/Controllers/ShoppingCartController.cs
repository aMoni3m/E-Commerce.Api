using E_Commerce.Api.DTOs.ShoppingCartDTOs;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCartByCustomerId([FromRoute] int customerId)
        {
            var response = await _shoppingCartService.GetCartByCustomerIdAsync(customerId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO addToCartDTO)
        {
            var response = await _shoppingCartService.AddToCartAsync(addToCartDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("item")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDTO updateCartItemDTO)
        {
            var response = await _shoppingCartService.UpdateCartItemAsync(updateCartItemDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("item")]
        public async Task<IActionResult> RemoveCartItem([FromBody] RemoveCartItemDTO removeCartItemDTO)
        {
            var response = await _shoppingCartService.RemoveCartItemAsync(removeCartItemDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("customer/{customerId}")]
        public async Task<IActionResult> ClearCart([FromRoute] int customerId)
        {
            var response = await _shoppingCartService.ClearCartAsync(customerId);
            return StatusCode(response.StatusCode, response);
        }
    }
}