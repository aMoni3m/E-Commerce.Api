using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> GetCustomerByEmail(String email);

        Task Add(Customer customer);
    }
}