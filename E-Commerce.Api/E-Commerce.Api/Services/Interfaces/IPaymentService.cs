using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.PaymentDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponseDTO>> ProcessPaymentAsync(PaymentRequestDTO paymentRequest);
        Task<ApiResponse<PaymentResponseDTO>> GetPaymentByIdAsync(int paymentId);
        Task<ApiResponse<PaymentResponseDTO>> GetPaymentByOrderIdAsync(int orderId);
        Task<ApiResponse<ConfirmationResponseDTO>> UpdatePaymentStatusAsync(PaymentStatusUpdateDTO statusUpdate);
        Task<ApiResponse<ConfirmationResponseDTO>> CompleteCODPaymentAsync(CODPaymentUpdateDTO codPaymentUpdateDTO);
    }
}