using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CustomerDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ApiResponse<CustomerResponseDTO>> RegisterCustomer(CustomerRegistrationDTO customerRegistration);

        Task<ApiResponse<CustomerResponseDTO>> FindCustomer(int id);

        Task<ApiResponse<ConfirmationResponseDTO>> UpdateCustomer(CustomerUpdateDTO customerUpdateDTO);

        Task<ApiResponse<PaginatedResultDto<CustomerResponseDTO>>> AllCustomer(int pageSize, int pageNumber);

        Task<ApiResponse<ConfirmationResponseDTO>> DeleteCustomer(int id);
    }
}