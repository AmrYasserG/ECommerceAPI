using AutoMapper;
using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.Common.Enums;
using ECommerceAPI.Common.Models;
using ECommerceAPI.DAL.UnitOfWork;

namespace ECommerceAPI.BLL.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public OrderManager(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<(OrderDto? Order, string? Error)> PlaceOrderAsync(string userId)
        {
            var cartItems = (await _uow.Cart.GetUserCartAsync(userId)).ToList();

            if (cartItems.Count == 0)
                return (null, "Cart is empty.");

            // Validate stock for every item before doing anything
            foreach (var ci in cartItems)
            {
                if (ci.Product.Stock < ci.Quantity)
                    return (null, $"Insufficient stock for '{ci.Product.Name}'. Available: {ci.Product.Stock}, Requested: {ci.Quantity}.");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList()
            };

            order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

            // Decrement stock for each ordered product
            foreach (var ci in cartItems)
                ci.Product.Stock -= ci.Quantity;

            await _uow.Orders.AddAsync(order);
            await _uow.Cart.ClearUserCartAsync(userId);
            await _uow.SaveChangesAsync();

            var created = await _uow.Orders.GetOrderWithItemsAsync(order.Id, userId);
            return (_mapper.Map<OrderDto>(created!), null);
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _uow.Orders.GetUserOrdersAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(string userId, int orderId)
        {
            var order = await _uow.Orders.GetOrderWithItemsAsync(orderId, userId);
            return order is null ? null : _mapper.Map<OrderDto>(order);
        }
    }
}
