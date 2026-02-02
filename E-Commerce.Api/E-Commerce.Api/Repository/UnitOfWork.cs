using E_Commerce.Api.Data;
using E_Commerce.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce.Api.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IDbContextTransaction _transaction;

        public IProductRepository Products { get; }

        public ICategoryRepository Categorys { get; }

        public IAddressRepository Addresss { get; }

        public IOrderRepository Orders { get; }

        public IRefundRepository Refunds { get; }

        public IShoppingCartRepository ShoppingCarts { get; }

        public ICancellationRepository Cancellations { get; }

        public IPaymentRepository Payments { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new ProductRepository(context);
            Categorys = new CategoryRepository(context);
            Addresss = new AddressRepository(context);
            Orders = new OrderRepository(context);
            Refunds = new RefundRepository(context);
            ShoppingCarts = new ShoppingCartRepository(context);
            Cancellations = new CancellationRepository(context);
            Payments = new PaymentRepository(context);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_transaction != null)
                return _transaction;

            _transaction = await _context.Database.BeginTransactionAsync();

            return _transaction;
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            await _context.SaveChangesAsync();

            await _transaction.CommitAsync();

            await DisposeTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to rollback.");

            await _transaction.RollbackAsync();

            await DisposeTransactionAsync();
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();

            _context?.Dispose();
        }
    }
}