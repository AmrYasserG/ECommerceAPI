using ECommerceAPI.Common.Models;
using ECommerceAPI.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.DAL.Repositories
{
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CartItem>> GetUserCartAsync(string userId)
            => await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

        public async Task<CartItem?> GetCartItemAsync(string userId, int productId)
            => await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        public async Task ClearUserCartAsync(string userId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();
            _context.CartItems.RemoveRange(items);
        }
    }
}
