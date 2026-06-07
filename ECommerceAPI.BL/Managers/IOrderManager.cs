using ECommerceAPI.BLL.DTOs;

namespace ECommerceAPI.BLL.Managers
{
    public interface IOrderManager
    {
        Task<(OrderDto? Order, string? Error)> PlaceOrderAsync(string userId);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto?> GetOrderByIdAsync(string userId, int orderId);
    }
}
