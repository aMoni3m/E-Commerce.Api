using AutoMapper;
using E_Commerce.Api.DTOs.AdressDTOs;
using E_Commerce.Api.DTOs.ProductDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();
            CreateMap<ProductStatusUpdateDTO, Product>();
            CreateMap<Product, ProductResponseDTO>();
        }
    }
}