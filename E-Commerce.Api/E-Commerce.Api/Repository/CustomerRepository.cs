using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_Commerce.Api.Data;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CustomerDTOs;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomerRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> GetCustomerByEmail(string email)
        {
            return await _context.Customers.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task Add(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetCustomerByID(int id)
        {
            Customer? customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return null;

            return customer;
        }

        public async Task Update(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResultDto<CustomerResponseDTO>> AllCustomer(
            int pageSize,
            int pageNumber
            )
        {
            var totalcount = await _context.Customers.CountAsync();
            var query = _context.Customers.AsNoTracking();

            var items = await query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CustomerResponseDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PaginatedResultDto<CustomerResponseDTO>(
                pageNumber, pageSize, totalcount, items
                );
        }

        public async Task DeleteCustoemr(int id)
        {
            Customer? customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}