using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<bool> ProductNameExistsAsync(string name, int? excludeId = null);

        Task<bool> CategoryExistsAsync(int categoryId);

        Task<Product> GetProductByIdAsync(int id);

        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
    }
}