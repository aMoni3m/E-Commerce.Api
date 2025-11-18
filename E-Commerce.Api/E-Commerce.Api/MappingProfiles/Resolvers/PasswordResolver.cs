using AutoMapper;
using E_Commerce.Api.DTOs.CustomerDTOs;
using E_Commerce.Api.Models;

namespace E_Commerce.Api.MappingProfiles.Resolvers
{
    public class PasswordResolver : IValueResolver<CustomerRegistrationDTO, Customer, string>
    {
        public string Resolve(CustomerRegistrationDTO source,
            Customer destinaion, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.Password))
            {
                return null;
            }

            return BCrypt.Net.BCrypt.HashPassword(source.Password);
        }
    }
}