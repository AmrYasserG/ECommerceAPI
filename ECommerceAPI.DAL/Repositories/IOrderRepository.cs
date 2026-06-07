using ECommerceAPI.Common.Models;

namespace ECommerceAPI.DAL.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderWithItemsAsync(int orderId, string userId);
    }
}
