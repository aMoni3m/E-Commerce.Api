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

        public async Task<ApiResponse<List<CustomerResponseDTO>>> AllCustomer()
        {
            try
            {
                List<Customer> Customers = await _customerRepository.AllCustomer();

                List<CustomerResponseDTO> customersDTO = _mapper.Map<List<CustomerResponseDTO>>(Customers);

                return new ApiResponse<List<CustomerResponseDTO>>(200, customersDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CustomerResponseDTO>>(500,
                     $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteCustomer(int id)
        {
            try
            {
                await _customerRepository.DeleteCustoemr(id);
                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Customer with Id {id} deleted successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500,

                     $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CustomerResponseDTO>> FindCustomer(int id)
        {
            try
            {
                Customer? customer = await _customerRepository.GetCustomerByID(id);
                if (customer == null)
                {
                    return new ApiResponse<CustomerResponseDTO>(400, "Invalid input");
                }
                CustomerResponseDTO response = _mapper.Map<CustomerResponseDTO>(customer);
                return new ApiResponse<CustomerResponseDTO>(200, response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CustomerResponseDTO>(500,
                     $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
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

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateCustomer(CustomerUpdateDTO customerUpdateDTO)
        {
            try
            {
                Customer? customer = await _customerRepository.GetCustomerByID(customerUpdateDTO.CustomerId);
                if (customer == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Customer not found");
                }

                _mapper.Map(customerUpdateDTO, customer);

                await _customerRepository.Update(customer);

                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Customer with Id {customerUpdateDTO.CustomerId} updated successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500,
                    $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
    }
}