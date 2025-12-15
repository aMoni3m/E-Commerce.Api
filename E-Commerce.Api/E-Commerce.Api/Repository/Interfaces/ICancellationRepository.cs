using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface ICancellationRepository
    {
        Task<Order?> GetOrderForCancellationAsync(int orderId, int customerId);
        Task<Cancellation?> GetCancellationByOrderIdAsync(int orderId);
        Task CreateCancellationAsync(Cancellation cancellation);
        Task<Cancellation?> GetCancellationByIdAsync(int id);
        Task<List<Cancellation>> GetAllCancellationsWithOrderAsync();
        Task<Cancellation?> GetCancellationWithOrderAndCustomerAsync(int cancellationId);
        Task<List<OrderItem>> GetOrderItemsWithProductByOrderIdAsync(int orderId);
        Task UpdateCancellationAsync(Cancellation cancellation);
        Task UpdateOrderAsync(Order order);
        Task UpdateProductAsync(Product product);
        Task SaveChangesAsync();
    }
}