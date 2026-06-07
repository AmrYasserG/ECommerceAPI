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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderManager _manager;

        public OrdersController(IOrderManager manager)
        {
            _manager = manager;
        }

        private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderDto>>> PlaceOrder()
        {
            if (UserId is null) return Unauthorized(ApiResponse<OrderDto>.FailResult("Unauthorized."));
            var (order, error) = await _manager.PlaceOrderAsync(UserId);
            if (order is null)
                return BadRequest(ApiResponse<OrderDto>.FailResult(error ?? "Could not place order."));
            return Ok(ApiResponse<OrderDto>.SuccessResult(order, "Order placed successfully."));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders()
        {
            if (UserId is null) return Unauthorized(ApiResponse<IEnumerable<OrderDto>>.FailResult("Unauthorized."));
            var orders = await _manager.GetUserOrdersAsync(UserId);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById([FromRoute] int id)
        {
            if (UserId is null) return Unauthorized(ApiResponse<OrderDto>.FailResult("Unauthorized."));
            var order = await _manager.GetOrderByIdAsync(UserId, id);
            if (order is null)
                return NotFound(ApiResponse<OrderDto>.FailResult($"Order with id {id} was not found."));
            return Ok(ApiResponse<OrderDto>.SuccessResult(order));
        }
    }
}
