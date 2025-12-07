using E_Commerce.Api.Models;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<Address> GetAddressByIdAsync(int addressId);
        Task<Product> GetProductByIdAsync(int productId);
        Task UpdateProductAsync(Product product);
        Task<Cart> GetActiveCartByCustomerIdAsync(int customerId);
        Task UpdateCartAsync(Cart cart);
    }
}

