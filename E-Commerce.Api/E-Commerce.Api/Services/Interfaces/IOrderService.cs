using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.OrderDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderResponseDTO>> CreateOrderAsync(OrderCreateDTO orderDto);
        Task<ApiResponse<OrderResponseDTO>> GetOrderByIdAsync(int orderId);
        Task<ApiResponse<ConfirmationResponseDTO>> UpdateOrderStatusAsync(OrderStatusUpdateDTO statusDto);
        Task<ApiResponse<List<OrderResponseDTO>>> GetAllOrdersAsync();
        Task<ApiResponse<List<OrderResponseDTO>>> GetOrdersByCustomerAsync(int customerId);
        Task<ApiResponse<ConfirmationResponseDTO>> DeleteOrderAsync(int orderId);
    }
}

