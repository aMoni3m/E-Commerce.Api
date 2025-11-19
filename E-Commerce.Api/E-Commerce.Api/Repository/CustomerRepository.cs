using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<List<Customer>> AllCustomer()
        {
            List<Customer> result = await _context.Customers.AsNoTracking().ToListAsync();
            return result;
        }

        public async Task DeleteCustoemr(int id)
        {
            Customer? customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}