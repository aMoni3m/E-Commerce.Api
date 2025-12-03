using E_Commerce.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Api.DTOs.OrderDTOs
{
    public class OrderStatusUpdateDTO
    {
        [Required(ErrorMessage = "OrderId is Required")]
        public int OrderId { get; set; }

        [Required]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid Order Status.")]
        public OrderStatus OrderStatus { get; set; }
    }
}