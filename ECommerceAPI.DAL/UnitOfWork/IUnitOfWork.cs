using ECommerceAPI.DAL.Repositories;

namespace ECommerceAPI.DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ICartRepository Cart { get; }
        IOrderRepository Orders { get; }
        Task<int> SaveChangesAsync();
    }
}
