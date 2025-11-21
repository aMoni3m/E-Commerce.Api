using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> CategoryIsExistAsync(string categoryName);

        Task CreateCategoryAsync(Category category);

        Task<Category> GetCategoryByIdAsync(int id);

        Task UpdateCategoryAsync(Category category);

        Task<List<Category>> GetCategoriesAsync();

        Task DeleteCategoryAsync(Category category);
    }
}