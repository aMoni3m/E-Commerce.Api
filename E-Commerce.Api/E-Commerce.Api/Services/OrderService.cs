using AutoMapper;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.OrderDTOs;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;
using System.Security.Cryptography;

namespace E_Commerce.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        private static readonly Dictionary<OrderStatus, List<OrderStatus>> AllowedStatusTransitions = new()
        {
            { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Canceled } },
            { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Canceled } },
            { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
            { OrderStatus.Delivered, new List<OrderStatus>() }, 
            { OrderStatus.Canceled, new List<OrderStatus>() }   
        };

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<OrderResponseDTO>> CreateOrderAsync(OrderCreateDTO orderDto)
        {
            try
            {
                var customer = await _orderRepository.GetCustomerByIdAsync(orderDto.CustomerId);
                if (customer == null)
                {
                    return new ApiResponse<OrderResponseDTO>(404, "Customer does not exist.");
                }

                var billingAddress = await _orderRepository.GetAddressByIdAsync(orderDto.BillingAddressId);
                if (billingAddress == null || billingAddress.CustomerId != orderDto.CustomerId)
                {
                    return new ApiResponse<OrderResponseDTO>(400, "Billing Address is invalid or does not belong to the customer.");
                }

                var shippingAddress = await _orderRepository.GetAddressByIdAsync(orderDto.ShippingAddressId);
                if (shippingAddress == null || shippingAddress.CustomerId != orderDto.CustomerId)
                {
                    return new ApiResponse<OrderResponseDTO>(400, "Shipping Address is invalid or does not belong to the customer.");
                }

                decimal totalBaseAmount = 0;
                decimal totalDiscountAmount = 0;
                decimal shippingCost = 10.00m; 
                decimal totalAmount = 0;

                string orderNumber = GenerateOrderNumber();

                var orderItems = new List<OrderItem>();

                foreach (var itemDto in orderDto.OrderItems)
                {
                    var product = await _orderRepository.GetProductByIdAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        return new ApiResponse<OrderResponseDTO>(404, $"Product with ID {itemDto.ProductId} does not exist.");
                    }
                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        return new ApiResponse<OrderResponseDTO>(400, $"Insufficient stock for product {product.Name}.");
                    }

                    decimal basePrice = itemDto.Quantity * product.Price;
                    decimal discount = (product.DiscountPercentage / 100.0m) * basePrice;
                    decimal totalPrice = basePrice - discount;

                    var orderItem = _mapper.Map<OrderItem>(itemDto);
                    orderItem.UnitPrice = product.Price;
                    orderItem.Discount = discount;
                    orderItem.TotalPrice = totalPrice;

                    orderItems.Add(orderItem);

                    totalBaseAmount += basePrice;
                    totalDiscountAmount += discount;

                    product.StockQuantity -= itemDto.Quantity;
                    await _orderRepository.UpdateProductAsync(product);
                }

                totalAmount = totalBaseAmount - totalDiscountAmount + shippingCost;

                var order = new Order
                {
                    OrderNumber = orderNumber,
                    CustomerId = orderDto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    BillingAddressId = orderDto.BillingAddressId,
                    ShippingAddressId = orderDto.ShippingAddressId,
                    TotalBaseAmount = totalBaseAmount,
                    TotalDiscountAmount = totalDiscountAmount,
                    ShippingCost = shippingCost,
                    TotalAmount = totalAmount,
                    OrderStatus = OrderStatus.Pending,
                    OrderItems = orderItems
                };

                await _orderRepository.CreateOrderAsync(order);

                var savedOrder = await _orderRepository.GetOrderByIdAsync(order.Id);

                var orderResponse = _mapper.Map<OrderResponseDTO>(savedOrder);

                return new ApiResponse<OrderResponseDTO>(200, orderResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<OrderResponseDTO>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
                }

                var orderResponse = _mapper.Map<OrderResponseDTO>(order);

                return new ApiResponse<OrderResponseDTO>(200, orderResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateOrderStatusAsync(OrderStatusUpdateDTO statusDto)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(statusDto.OrderId);
                if (order == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Order not found.");
                }

                var currentStatus = order.OrderStatus;
                var newStatus = statusDto.OrderStatus;

                if (!AllowedStatusTransitions.TryGetValue(currentStatus, out var allowedStatuses))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(500, "Current order status is invalid.");
                }

                if (!allowedStatuses.Contains(newStatus))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, $"Cannot change order status from {currentStatus} to {newStatus}.");
                }

                order.OrderStatus = newStatus;
                await _orderRepository.UpdateOrderAsync(order);

                var confirmation = new ConfirmationResponseDTO
                {
                    Message = $"Order Status with Id {statusDto.OrderId} updated successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<OrderResponseDTO>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                var orderList = _mapper.Map<List<OrderResponseDTO>>(orders);
                return new ApiResponse<List<OrderResponseDTO>>(200, orderList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<OrderResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<OrderResponseDTO>>> GetOrdersByCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _orderRepository.GetCustomerByIdAsync(customerId);
                if (customer == null)
                {
                    return new ApiResponse<List<OrderResponseDTO>>(404, "Customer not found.");
                }
                var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
                var orderList = _mapper.Map<List<OrderResponseDTO>>(orders);

                return new ApiResponse<List<OrderResponseDTO>>(200, orderList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<OrderResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }


        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteOrderAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Order not found.");
                }
                await _orderRepository.DeleteOrderAsync(order);
                var confirmation = new ConfirmationResponseDTO
                {
                    Message = $"Order with Id {orderId} deleted successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        #region Helper Methods
        
        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")}-{RandomNumber(1000, 9999)}";
        }

        private int RandomNumber(int min, int max)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return Math.Abs(BitConverter.ToInt32(bytes, 0) % (max - min + 1)) + min;
            }
        }
        #endregion
    }
}
