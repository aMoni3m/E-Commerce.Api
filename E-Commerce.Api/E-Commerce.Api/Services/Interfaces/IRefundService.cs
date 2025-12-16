using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.RefundDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface IRefundService
    {
        Task<ApiResponse<List<PendingRefundResponseDTO>>> GetEligibleRefundsAsync();
        Task<ApiResponse<RefundResponseDTO>> ProcessRefundAsync(RefundRequestDTO refundRequest);
        Task<ApiResponse<ConfirmationResponseDTO>> UpdateRefundStatusAsync(RefundStatusUpdateDTO statusUpdate);
        Task<ApiResponse<RefundResponseDTO>> GetRefundByIdAsync(int id);
        Task<ApiResponse<List<RefundResponseDTO>>> GetAllRefundsAsync();
    }
}
