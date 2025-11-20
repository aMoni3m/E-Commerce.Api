using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAddressAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(Address address)
        {
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }

        public async Task<Address> GetAddressByIdAsync(int customerId, int addressId)
        {
            Address? address = await _context.Addresses.FirstOrDefaultAsync(add => add.CustomerId == customerId && add.Id == addressId);
            return address;
        }

        public async Task<List<Address>> GetAllAddressesAsync()
        {
            List<Address> addresses = await _context.Addresses.AsNoTracking().ToListAsync();

            return addresses;
        }

        public async Task UpdateAddressAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }
    }
}