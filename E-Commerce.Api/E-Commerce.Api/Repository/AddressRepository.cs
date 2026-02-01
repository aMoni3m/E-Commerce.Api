using E_Commerce.Api.Data;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Repository
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Address> GetAddressByIdAsync(int customerId, int addressId)
        {
            Address? address = await _context.Addresses.FirstOrDefaultAsync(add => add.CustomerId == customerId && add.Id == addressId);
            return address;
        }
    }
}