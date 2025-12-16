using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class RefundRepository : IRefundRepository
    {
        private readonly ApplicationDbContext _context;

        public RefundRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cancellation>> GetEligibleCancellationsForRefundAsync()
        {
            return await _context.Cancellations
                .Include(c => c.Order)
                    .ThenInclude(o => o.Payment)
                .Where(c => c.Status == CancellationStatus.Approved && c.Refund == null
                            && c.Order.Payment.PaymentMethod.ToLower() != "cod")
                .ToListAsync();
        }

        public async Task<Cancellation?> GetCancellationWithOrderPaymentAndCustomerAsync(int cancellationId)
        {
            return await _context.Cancellations
                .Include(c => c.Order)
                    .ThenInclude(o => o.Payment)
                .Include(c => c.Order)
                    .ThenInclude(o => o.Customer)
                .FirstOrDefaultAsync(c => c.Id == cancellationId);
        }

        public async Task<Refund?> GetRefundByCancellationIdAsync(int cancellationId)
        {
            return await _context.Refunds
                .FirstOrDefaultAsync(r => r.CancellationId == cancellationId);
        }

        public async Task<Refund> CreateRefundAsync(Refund refund)
        {
            await _context.Refunds.AddAsync(refund);
            return refund;
        }

        public async Task<Refund?> GetRefundByIdAsync(int id)
        {
            return await _context.Refunds
                .Include(r => r.Cancellation)
                    .ThenInclude(c => c.Order)
                        .ThenInclude(o => o.Payment)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Refund?> GetRefundWithDetailsAsync(int refundId)
        {
            return await _context.Refunds
                .Include(r => r.Cancellation)
                    .ThenInclude(c => c.Order)
                        .ThenInclude(o => o.Customer)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == refundId);
        }

        public async Task<List<Refund>> GetAllRefundsAsync()
        {
            return await _context.Refunds
                .Include(r => r.Cancellation)
                    .ThenInclude(c => c.Order)
                        .ThenInclude(o => o.Payment)
                .ToListAsync();
        }

        public async Task UpdateRefundAsync(Refund refund)
        {
            _context.Refunds.Update(refund);
            await Task.CompletedTask;
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
