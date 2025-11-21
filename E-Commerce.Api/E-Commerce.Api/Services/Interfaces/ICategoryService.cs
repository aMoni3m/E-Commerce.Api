using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CategoryDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryResponseDTO>> CreateCategoryAsync(CategoryCreateDTO categoryCreateDTO);

        Task<ApiResponse<CategoryResponseDTO>> GetCategoryByIdAsync(int id);

        Task<ApiResponse<ConfirmationResponseDTO>> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDTO);

        Task<ApiResponse<ConfirmationResponseDTO>> DeleteCategoryAsync(int id);

        Task<ApiResponse<List<CategoryResponseDTO>>> GetAllCategoryAsync();
    }
}