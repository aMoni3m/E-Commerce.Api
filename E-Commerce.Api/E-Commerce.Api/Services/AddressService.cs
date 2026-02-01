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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public AddressService(IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<AddressResponseDTO>>> AllAddressesAsync()
        {
            try
            {
                List<Address> addresses = await _unitOfWork.Addresss.GetAllAsync();

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

                await _unitOfWork.Addresss.CreateAsync(address);
                await _unitOfWork.SaveChangesAsync();
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
                Address address = await _unitOfWork.Addresss.GetAddressByIdAsync(addressDeleteDTO.CustomerId, addressDeleteDTO.AddressId);
                if (address == null)
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Address not Found");

                _unitOfWork.Addresss.Delete(address);
                await _unitOfWork.SaveChangesAsync();
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
                Address address = await _unitOfWork.Addresss.GetAddressByIdAsync(addressUpdateDTO.CustomerId, addressUpdateDTO.Id);
                if (address == null)
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Customer or Address not found");

                _mapper.Map(addressUpdateDTO, address);

                _unitOfWork.Addresss.Update(address);
                await _unitOfWork.SaveChangesAsync();
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