using ECommerceAPI.DAL.Identity;

namespace ECommerceAPI.BLL.Services
{
    public interface ITokenService
    {
        string GenerateToken(AppUser user, IList<string> roles);
    }
}
