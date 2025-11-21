using AutoMapper;
using E_Commerce.Api.DTOs.CategoryDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<Category, CategoryResponseDTO>();
        }
    }
}