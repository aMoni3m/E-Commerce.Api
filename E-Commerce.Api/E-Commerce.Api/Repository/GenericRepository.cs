using E_Commerce.Api.Data;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _dbSet = context.Set<T>();
            _context = context;
        }

        public async Task CreateAsync(T data)
        {
            await _dbSet.AddAsync(data);
        }

        public void Delete(T Adrress)
        {
            _dbSet.Remove(Adrress);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public void Update(T data)
        {
            _dbSet.Update(data);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}