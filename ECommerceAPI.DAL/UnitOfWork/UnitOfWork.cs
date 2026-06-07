using ECommerceAPI.DAL.Context;
using ECommerceAPI.DAL.Repositories;

namespace ECommerceAPI.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public ICartRepository Cart { get; }
        public IOrderRepository Orders { get; }

        public UnitOfWork(
            AppDbContext context,
            IProductRepository products,
            ICategoryRepository categories,
            ICartRepository cart,
            IOrderRepository orders)
        {
            _context = context;
            Products = products;
            Categories = categories;
            Cart = cart;
            Orders = orders;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
