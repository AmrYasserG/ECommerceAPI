using Microsoft.AspNetCore.Http;

namespace ECommerceAPI.Services
{
    public interface IImageService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
