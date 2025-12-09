using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.PaymentDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponseDTO>> ProcessPaymentAsync(PaymentRequestDTO paymentRequest);
    }
}