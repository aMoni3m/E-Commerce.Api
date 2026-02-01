using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task CreateAsync(T data);

        void Update(T data);

        Task<List<T>> GetAllAsync();

        void Delete(T Adrress);
    }
}