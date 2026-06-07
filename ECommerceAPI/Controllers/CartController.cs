using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.BLL.Managers;
using ECommerceAPI.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthenticatedUser")]
    public class CartController : ControllerBase
    {
        private readonly ICartManager _manager;

        public CartController(ICartManager manager)
        {
            _manager = manager;
        }

        private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CartItemDto>>>> GetCart()
        {
            if (UserId is null) return Unauthorized(ApiResponse<IEnumerable<CartItemDto>>.FailResult("Unauthorized."));
            var items = await _manager.GetCartAsync(UserId);
            return Ok(ApiResponse<IEnumerable<CartItemDto>>.SuccessResult(items));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> AddToCart([FromBody] AddToCartDto dto)
        {
            if (UserId is null) return Unauthorized(ApiResponse<CartItemDto>.FailResult("Unauthorized."));
            var item = await _manager.AddToCartAsync(UserId, dto);
            return Ok(ApiResponse<CartItemDto>.SuccessResult(item, "Item added to cart."));
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> UpdateCartItem([FromBody] UpdateCartDto dto)
        {
            if (UserId is null) return Unauthorized(ApiResponse<CartItemDto>.FailResult("Unauthorized."));
            var item = await _manager.UpdateCartItemAsync(UserId, dto);
            if (item is null)
                return NotFound(ApiResponse<CartItemDto>.FailResult("Cart item not found."));
            return Ok(ApiResponse<CartItemDto>.SuccessResult(item, "Cart item updated."));
        }

        [HttpDelete("{productId:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveFromCart([FromRoute] int productId)
        {
            if (UserId is null) return Unauthorized(ApiResponse<bool>.FailResult("Unauthorized."));
            var removed = await _manager.RemoveFromCartAsync(UserId, productId);
            if (!removed)
                return NotFound(ApiResponse<bool>.FailResult("Cart item not found."));
            return Ok(ApiResponse<bool>.SuccessResult(true, "Item removed from cart."));
        }
    }
}
