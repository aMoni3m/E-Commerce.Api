using AutoMapper;
using E_Commerce.Api.DTOs.CustomerDTOs;
using E_Commerce.Api.MappingProfiles.Resolvers;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.MappingProfiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerRegistrationDTO, Customer>().ForMember(
                dest => dest.Password, opt => opt.MapFrom<PasswordResolver>());

            CreateMap<Customer, CustomerResponseDTO>();

            CreateMap<CustomerUpdateDTO, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opts =>

                    opts.Condition((src, dest, Member) =>
                    {
                        if (Member == null)
                            return false;

                        if (Member is string str)
                            return !string.IsNullOrWhiteSpace(str);
                        return true;
                    }
                    ));
        }
    }
}