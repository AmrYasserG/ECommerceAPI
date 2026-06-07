using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.DAL.Identity
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
