using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.BLL.Services;
using ECommerceAPI.DAL.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.BLL.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthManager(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<(AuthResponseDto? Response, IList<string>? Errors)> RegisterAsync(RegisterDto dto)
        {
            var user = new AppUser
            {
                FullName = dto.FullName,
                Email    = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (null, result.Errors.Select(e => e.Description).ToList());

            await _userManager.AddToRoleAsync(user, "User");
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return (new AuthResponseDto
            {
                Token    = token,
                Email    = user.Email!,
                FullName = user.FullName,
                Roles    = roles
            }, null);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new AuthResponseDto
            {
                Token    = token,
                Email    = user.Email!,
                FullName = user.FullName,
                Roles    = roles
            };
        }
    }
}
