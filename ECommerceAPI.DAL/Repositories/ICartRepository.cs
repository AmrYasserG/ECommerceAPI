using ECommerceAPI.Common.Models;

namespace ECommerceAPI.DAL.Repositories
{
    public interface ICartRepository : IGenericRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetUserCartAsync(string userId);
        Task<CartItem?> GetCartItemAsync(string userId, int productId);
        Task ClearUserCartAsync(string userId);
    }
}
