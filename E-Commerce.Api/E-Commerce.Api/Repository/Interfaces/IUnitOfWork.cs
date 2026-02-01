using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce.Api.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categorys { get; }
        IAddressRepository Addresss { get; }
        IOrderRepository Orders { get; }
        IRefundRepository Refunds { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        ICancellationRepository Cancellations { get; }
        IPaymentRepository Payments { get; }

        Task SaveChangesAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}