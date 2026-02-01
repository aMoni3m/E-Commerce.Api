using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CategoryIsExistAsync(string categoryName)
        {
            return await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower());
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            Category category = await _context.Categories.FindAsync(id);
            return category;
        }
    }
}