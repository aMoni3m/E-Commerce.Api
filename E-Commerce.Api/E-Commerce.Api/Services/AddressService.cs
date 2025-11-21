using AutoMapper;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.AdressDTOs;
using E_Commerce.Api.DTOs.CustomerDTOs;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;
using System.Collections.Generic;

namespace E_Commerce.Api.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, ICustomerRepository customerRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<AddressResponseDTO>>> AllAddressesAsync()
        {
            try
            {
                List<Address> addresses = await _addressRepository.GetAllAddressesAsync();

                if (addresses == null) return new ApiResponse<List<AddressResponseDTO>>(400, "");

                List<AddressResponseDTO> AddressesDTO = _mapper.Map<List<AddressResponseDTO>>(addresses);

                return new ApiResponse<List<AddressResponseDTO>>(200, AddressesDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AddressResponseDTO>>(500,
                       $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AddressResponseDTO>> CreateAddressAsync(AddressCreateDTO addressCreateDTO)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByID(addressCreateDTO.CustomerId);
                if (customer == null)
                    return new ApiResponse<AddressResponseDTO>(400, "Customer not Found");

                Address address = _mapper.Map<Address>(addressCreateDTO);

                await _addressRepository.CreateAddressAsync(address);

                AddressResponseDTO addressResponseDTO = _mapper.Map<AddressResponseDTO>(address);
                return new ApiResponse<AddressResponseDTO>(201, addressResponseDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddressResponseDTO>(500,
                    $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteAddressAsync(AddressDeleteDTO addressDeleteDTO)
        {
            try
            {
                Address address = await _addressRepository.GetAddressByIdAsync(addressDeleteDTO.CustomerId, addressDeleteDTO.AddressId);
                if (address == null)
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Address not Found");

                await _addressRepository.DeleteAddressAsync(address);
                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Address with Id {addressDeleteDTO.AddressId} deleted successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500,
                       $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateAddressAsync(AddressUpdateDTO addressUpdateDTO)
        {
            try
            {
                Address address = await _addressRepository.GetAddressByIdAsync(addressUpdateDTO.CustomerId, addressUpdateDTO.Id);
                if (address == null)
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Customer or Address not found");

                _mapper.Map(addressUpdateDTO, address);

                await _addressRepository.UpdateAddressAsync(address);
                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Address with Id {addressUpdateDTO.Id} updated successfully."
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