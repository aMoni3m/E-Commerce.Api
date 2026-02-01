using E_Commerce.Api.DTOs.AdressDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        Task<Address> GetAddressByIdAsync(int customerId, int addressId);
    }
}