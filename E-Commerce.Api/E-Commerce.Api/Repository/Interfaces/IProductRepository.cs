using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> ProductNameExistsAsync(string name, int? excludeId = null);

        Task<bool> CategoryExistsAsync(int categoryId);

        Task CreateProductAsync(Product product);

        Task UpdateProductAsync(Product product);

        Task<Product> GetProductByIdAsync(int id);

        Task<List<Product>> GetAllProductsAsync();

        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);

        Task DeleteProductAsync(Product product);
    }
}