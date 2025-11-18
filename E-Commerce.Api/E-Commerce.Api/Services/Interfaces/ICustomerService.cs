using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CustomerDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ApiResponse<CustomerResponseDTO>> RegisterCustomer(CustomerRegistrationDTO customerRegistration);
    }
}