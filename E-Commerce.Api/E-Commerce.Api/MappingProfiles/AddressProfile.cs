using AutoMapper;
using E_Commerce.Api.DTOs.AdressDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.MappingProfiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<AddressCreateDTO, Address>();
            CreateMap<AddressUpdateDTO, Address>();
            CreateMap<Address, AddressResponseDTO>();
        }
    }
}