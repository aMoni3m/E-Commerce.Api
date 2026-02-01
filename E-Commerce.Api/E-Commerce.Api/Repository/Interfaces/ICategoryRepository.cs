using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> CategoryIsExistAsync(string categoryName);

        Task<Category> GetCategoryByIdAsync(int id);
    }
}