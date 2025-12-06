using E_Commerce.Api.DTOs.OrderDTOs;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderDto)
        {
            var response = await _orderService.CreateOrderAsync(orderDto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] OrderStatusUpdateDTO statusDto)
        {
            var response = await _orderService.UpdateOrderStatusAsync(statusDto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await _orderService.GetAllOrdersAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomer([FromRoute] int customerId)
        {
            var response = await _orderService.GetOrdersByCustomerAsync(customerId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            var response = await _orderService.DeleteOrderAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}

