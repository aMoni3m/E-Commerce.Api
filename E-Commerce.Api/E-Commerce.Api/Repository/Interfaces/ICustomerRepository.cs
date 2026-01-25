using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CustomerDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> GetCustomerByEmail(String email);

        Task Add(Customer customer);

        Task<Customer?> GetCustomerByID(int id);

        Task Update(Customer customer);

        Task<PaginatedResultDto<CustomerResponseDTO>> AllCustomer(int pageSize, int pageNumber);

        Task DeleteCustoemr(int id);
    }
}