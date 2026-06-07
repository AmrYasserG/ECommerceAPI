using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.BLL.Managers;
using ECommerceAPI.Common;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
        {
            var (response, errors) = await _authManager.RegisterAsync(dto);
            if (response is null)
                return BadRequest(ApiResponse<AuthResponseDto>.FailResult("Registration failed.", errors?.ToList()));
            return Ok(ApiResponse<AuthResponseDto>.SuccessResult(response, "Registration successful."));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto dto)
        {
            var response = await _authManager.LoginAsync(dto);
            if (response is null)
                return Unauthorized(ApiResponse<AuthResponseDto>.FailResult("Invalid email or password."));
            return Ok(ApiResponse<AuthResponseDto>.SuccessResult(response, "Login successful."));
        }
    }
}
