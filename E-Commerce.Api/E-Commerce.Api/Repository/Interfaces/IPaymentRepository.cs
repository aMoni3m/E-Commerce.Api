using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Order> GetOrderWithPaymentAsync(int orderId, int customerId);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
        Task<Payment> GetPaymentWithOrderAsync(int paymentId);
        Task<Payment> GetPaymentWithOrderByOrderIdAsync(int paymentId, int orderId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task UpdateOrderAsync(Order order);
        Task SaveChangesAsync();
    }
}
