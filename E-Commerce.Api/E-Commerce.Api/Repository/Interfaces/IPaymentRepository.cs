using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Order> GetOrderWithPaymentAsync(int orderId, int customerId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task UpdateOrderAsync(Order order);
        Task SaveChangesAsync();
    }
}
