using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IRefundRepository
    {
        Task<List<Cancellation>> GetEligibleCancellationsForRefundAsync();

        Task<Cancellation?> GetCancellationWithOrderPaymentAndCustomerAsync(int cancellationId);

        Task<Refund?> GetRefundByCancellationIdAsync(int cancellationId);

        Task<Refund> CreateRefundAsync(Refund refund);

        Task<Refund?> GetRefundByIdAsync(int id);

        Task<Refund?> GetRefundWithDetailsAsync(int refundId);

        Task<List<Refund>> GetAllRefundsAsync();

        void UpdateRefundAsync(Refund refund);

        void UpdatePaymentAsync(Payment payment);
    }
}