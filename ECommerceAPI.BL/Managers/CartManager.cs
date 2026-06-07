using AutoMapper;
using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.Common.Models;
using ECommerceAPI.DAL.UnitOfWork;

namespace ECommerceAPI.BLL.Managers
{
    public class CartManager : ICartManager
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CartManager(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartItemDto>> GetCartAsync(string userId)
        {
            var items = await _uow.Cart.GetUserCartAsync(userId);
            return _mapper.Map<IEnumerable<CartItemDto>>(items);
        }

        public async Task<CartItemDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var existing = await _uow.Cart.GetCartItemAsync(userId, dto.ProductId);
            if (existing is not null)
            {
                existing.Quantity += dto.Quantity;
                _uow.Cart.Update(existing);
            }
            else
            {
                var item = new CartItem
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                await _uow.Cart.AddAsync(item);
            }

            await _uow.SaveChangesAsync();
            var saved = await _uow.Cart.GetCartItemAsync(userId, dto.ProductId);
            return _mapper.Map<CartItemDto>(saved!);
        }

        public async Task<CartItemDto?> UpdateCartItemAsync(string userId, UpdateCartDto dto)
        {
            var item = await _uow.Cart.GetCartItemAsync(userId, dto.ProductId);
            if (item is null) return null;

            item.Quantity = dto.Quantity;
            _uow.Cart.Update(item);
            await _uow.SaveChangesAsync();

            return _mapper.Map<CartItemDto>(item);
        }

        public async Task<bool> RemoveFromCartAsync(string userId, int productId)
        {
            var item = await _uow.Cart.GetCartItemAsync(userId, productId);
            if (item is null) return false;

            _uow.Cart.Delete(item);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
