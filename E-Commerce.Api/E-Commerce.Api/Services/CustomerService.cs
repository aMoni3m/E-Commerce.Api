using AutoMapper;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CustomerDTOs;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;

namespace E_Commerce.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CustomerResponseDTO>> RegisterCustomer(CustomerRegistrationDTO customerRegistration)
        {
            try
            {
                bool result = await _customerRepository.GetCustomerByEmail(customerRegistration.Email);
                if (result)
                {
                    return new ApiResponse<CustomerResponseDTO>(400, "Email is already exist");
                }

                Customer customer = _mapper.Map<Customer>(customerRegistration);

                await _customerRepository.Add(customer);

                CustomerResponseDTO customerResponseDTO = _mapper.Map<CustomerResponseDTO>(customer);

                return new ApiResponse<CustomerResponseDTO>(201, customerResponseDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CustomerResponseDTO>(500,
                    $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
    }
}