using AutoMapper;
using E_Commerce.Api.DTOs.ShoppingCartDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.MappingProfiles
{
    public class ShoppingCartProfile : Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<Cart, CartResponseDTO>()
                .AfterMap((src, dest) =>
                {
                    decimal totalBasePrice = 0;
                    decimal totalDiscount = 0;
                    decimal totalAmount = 0;

                    if (dest.CartItems != null)
                    {
                        foreach (var item in dest.CartItems)
                        {
                            totalBasePrice += item.UnitPrice * item.Quantity;
                            totalDiscount += item.Discount * item.Quantity;
                            totalAmount += item.TotalPrice;
                        }
                    }

                    dest.TotalBasePrice = totalBasePrice;
                    dest.TotalDiscount = totalDiscount;
                    dest.TotalAmount = totalAmount;
                });

            CreateMap<CartItem, CartItemResponseDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
        }
    }
}

