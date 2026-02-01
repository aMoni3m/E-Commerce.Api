using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class CancellationRepository : ICancellationRepository
    {
        private readonly ApplicationDbContext _context;

        public CancellationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderForCancellationAsync(int orderId, int customerId)
        {
            return await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId);
        }

        public async Task<Cancellation?> GetCancellationByOrderIdAsync(int orderId)
        {
            return await _context.Cancellations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.OrderId == orderId);
        }

        public async Task CreateCancellationAsync(Cancellation cancellation)
        {
            await _context.Cancellations.AddAsync(cancellation);
        }

        public async Task<Cancellation?> GetCancellationByIdAsync(int id)
        {
            return await _context.Cancellations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cancellation>> GetAllCancellationsWithOrderAsync()
        {
            return await _context.Cancellations
                .AsNoTracking()
                .Include(c => c.Order)
                .ToListAsync();
        }

        public async Task<Cancellation?> GetCancellationWithOrderAndCustomerAsync(int cancellationId)
        {
            return await _context.Cancellations
                .Include(c => c.Order)
                    .ThenInclude(o => o.Customer)
                .FirstOrDefaultAsync(c => c.Id == cancellationId);
        }

        public async Task<List<OrderItem>> GetOrderItemsWithProductByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public void UpdateCancellationAsync(Cancellation cancellation)
        {
            _context.Cancellations.Update(cancellation);
        }

        public void UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
        }

        public void UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
        }
    }
}