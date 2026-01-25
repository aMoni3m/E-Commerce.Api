using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CancellationDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface ICancellationService
    {
        Task<ApiResponse<CancellationResponseDTO>> RequestCancellationAsync(CancellationRequestDTO cancellationRequest);
        Task<ApiResponse<CancellationResponseDTO>> GetCancellationByIdAsync(int id);
        Task<ApiResponse<List<CancellationResponseDTO>>> GetAllCancellationsAsync();
        Task<ApiResponse<ConfirmationResponseDTO>> UpdateCancellationStatusAsync(CancellationStatusUpdateDTO statusUpdate);
    }
}
