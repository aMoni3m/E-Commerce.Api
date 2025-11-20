using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.AdressDTOs;

namespace E_Commerce.Api.Services.Interfaces
{
    public interface IAddressService
    {
        Task<ApiResponse<AddressResponseDTO>> CreateAddressAsync(AddressCreateDTO addressCreateDTO);

        Task<ApiResponse<ConfirmationResponseDTO>> UpdateAddressAsync(AddressUpdateDTO addressUpdateDTO);

        Task<ApiResponse<List<AddressResponseDTO>>> AllAddressesAsync();

        Task<ApiResponse<ConfirmationResponseDTO>> DeleteAddressAsync(AddressDeleteDTO addressDeleteDTO);
    }
}