using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CategoryIsExistAsync(string categoryName)
        {
            return await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower());
        }

        public async Task CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            Category category = await _context.Categories.FindAsync(id);
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}