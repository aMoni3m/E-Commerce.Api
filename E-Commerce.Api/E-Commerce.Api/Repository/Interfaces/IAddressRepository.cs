using E_Commerce.Api.DTOs.AdressDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IAddressRepository
    {
        Task CreateAddressAsync(Address address);

        Task UpdateAddressAsync(Address address);

        Task<Address> GetAddressByIdAsync(int customerId, int addressId);

        Task<List<Address>> GetAllAddressesAsync();

        Task DeleteAddressAsync(Address Adrress);
    }
}