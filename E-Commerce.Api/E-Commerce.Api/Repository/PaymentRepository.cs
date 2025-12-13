using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderWithPaymentAsync(int orderId, int customerId)
        {
            return await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId);
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<Payment> GetPaymentWithOrderAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task<Payment> GetPaymentWithOrderByOrderIdAsync(int paymentId, int orderId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.OrderId == orderId);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            return payment;
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await Task.CompletedTask;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}