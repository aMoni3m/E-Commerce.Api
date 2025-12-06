using AutoMapper;
using E_Commerce.Api.DTOs.OrderDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderCreateDTO, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
                .ForMember(dest => dest.TotalBaseAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TotalDiscountAmount, opt => opt.Ignore())
                .ForMember(dest => dest.ShippingCost, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.OrderStatus, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.BillingAddress, opt => opt.Ignore())
                .ForMember(dest => dest.ShippingAddress, opt => opt.Ignore())
                .ForMember(dest => dest.Payment, opt => opt.Ignore())
                .ForMember(dest => dest.Cancellation, opt => opt.Ignore());

            CreateMap<OrderItemCreateDTO, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Discount, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()));

            CreateMap<OrderItem, OrderItemResponseDTO>();
        }
    }
}

