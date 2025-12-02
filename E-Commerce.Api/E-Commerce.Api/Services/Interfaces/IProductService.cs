using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.ProductDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductResponseDTO>> CreateProductAsync(ProductCreateDTO dto);

        Task<ApiResponse<ProductResponseDTO>> GetProductByIdAsync(int id);

        Task<ApiResponse<List<ProductResponseDTO>>> GetAllProductsAsync();

        Task<ApiResponse<List<ProductResponseDTO>>> GetProductsByCategoryAsync(int categoryId);

        Task<ApiResponse<ConfirmationResponseDTO>> UpdateProductAsync(ProductUpdateDTO dto);

        Task<ApiResponse<ConfirmationResponseDTO>> UpdateProductStatusAsync(ProductStatusUpdateDTO dto);

        Task<ApiResponse<ConfirmationResponseDTO>> DeleteProductAsync(int id);
    }
}