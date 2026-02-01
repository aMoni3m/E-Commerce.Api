using AutoMapper;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.AdressDTOs;
using E_Commerce.Api.DTOs.CategoryDTOs;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;

namespace E_Commerce.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CategoryResponseDTO>> CreateCategoryAsync(CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                if (await _categoryRepository.CategoryIsExistAsync(categoryCreateDTO.Name))
                    return new ApiResponse<CategoryResponseDTO>(400, "Category already exist");

                Category category = _mapper.Map<Category>(categoryCreateDTO);
                await _categoryRepository.CreateAsync(category);
                await _categoryRepository.SaveAsync();
                CategoryResponseDTO categoryResponseDTO = _mapper.Map<CategoryResponseDTO>(category);
                return new ApiResponse<CategoryResponseDTO>(201, categoryResponseDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponseDTO>(500,
                    $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteCategoryAsync(int id)
        {
            try
            {
                Category category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Category not found");
                _categoryRepository.Delete(category);
                await _categoryRepository.SaveAsync();

                return new ApiResponse<ConfirmationResponseDTO>(204, "");
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500,
                       $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CategoryResponseDTO>>> GetAllCategoryAsync()
        {
            try
            {
                List<Category> categories = await _categoryRepository.GetAllAsync();
                if (categories == null)
                    return new ApiResponse<List<CategoryResponseDTO>>(400, "categories not found");

                List<CategoryResponseDTO> ccategoriesResponse = _mapper.Map<List<CategoryResponseDTO>>(categories);

                return new ApiResponse<List<CategoryResponseDTO>>(200, ccategoriesResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryResponseDTO>>(500,
                       $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }

            throw new NotImplementedException();
        }

        public async Task<ApiResponse<CategoryResponseDTO>> GetCategoryByIdAsync(int id)
        {
            try
            {
                Category category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                    return new ApiResponse<CategoryResponseDTO>(400, "Category not found");

                CategoryResponseDTO categoryResponse = _mapper.Map<CategoryResponseDTO>(category);

                return new ApiResponse<CategoryResponseDTO>(200, categoryResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponseDTO>(500,
                       $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDTO)
        {
            try
            {
                Category category = await _categoryRepository.GetCategoryByIdAsync(categoryUpdateDTO.Id);
                if (category == null)
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Category not found");
                _mapper.Map(categoryUpdateDTO, category);
                _categoryRepository.Update(category);
                await _categoryRepository.SaveAsync();
                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Address with Id {categoryUpdateDTO.Id} updated successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500,
                       $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
            throw new NotImplementedException();
        }
    }
}