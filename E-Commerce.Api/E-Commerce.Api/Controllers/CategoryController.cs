using E_Commerce.Api.DTOs.AdressDTOs;
using E_Commerce.Api.DTOs.CategoryDTOs;
using E_Commerce.Api.Services;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> Allcategories()
        {
            var response = await _categoryService.GetAllCategoryAsync();

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
            var response = await _categoryService.CreateCategoryAsync(categoryCreateDTO);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDTO categoryUpdateDTO)
        {
            var response = await _categoryService.UpdateCategoryAsync(categoryUpdateDTO);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}