using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ProductNameExistsAsync(string name, int? excludeId = null)
        {
            return await _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower()
                            && (!excludeId.HasValue || p.Id != excludeId));
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsAvailable)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}