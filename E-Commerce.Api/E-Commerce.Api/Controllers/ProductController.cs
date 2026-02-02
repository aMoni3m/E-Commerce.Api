using E_Commerce.Api.DTOs.ProductDTOs;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO dto)
        {
            var response = await _productService.CreateProductAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("Category/{categoryId}")]
        [AllowAnonymous]
        [EnableRateLimiting("rateLimit")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var response = await _productService.GetProductsByCategoryAsync(categoryId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateDTO dto)
        {
            var response = await _productService.UpdateProductAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateProductStatus([FromBody] ProductStatusUpdateDTO dto)
        {
            var response = await _productService.UpdateProductStatusAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}