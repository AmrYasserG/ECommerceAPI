using ECommerceAPI.BLL.DTOs;

namespace ECommerceAPI.BLL.Managers
{
    public interface ICartManager
    {
        Task<IEnumerable<CartItemDto>> GetCartAsync(string userId);
        Task<CartItemDto> AddToCartAsync(string userId, AddToCartDto dto);
        Task<CartItemDto?> UpdateCartItemAsync(string userId, UpdateCartDto dto);
        Task<bool> RemoveFromCartAsync(string userId, int productId);
    }
}
