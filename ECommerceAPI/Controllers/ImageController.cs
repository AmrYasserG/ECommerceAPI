using ECommerceAPI.Common;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthenticatedUser")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<ApiResponse<string>>> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<string>.FailResult("No file was uploaded."));

            try
            {
                var url = await _imageService.UploadAsync(file);
                return Ok(ApiResponse<string>.SuccessResult(url, "Image uploaded successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.FailResult(ex.Message));
            }
        }
    }
}
