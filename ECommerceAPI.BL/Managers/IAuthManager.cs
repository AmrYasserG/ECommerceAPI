using ECommerceAPI.BLL.DTOs;

namespace ECommerceAPI.BLL.Managers
{
    public interface IAuthManager
    {
        Task<(AuthResponseDto? Response, IList<string>? Errors)> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
