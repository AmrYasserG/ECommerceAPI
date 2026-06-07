using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.BLL.Managers;
using ECommerceAPI.Common;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManager _manager;
        private readonly IImageService _imageService;

        public ProductsController(IProductManager manager, IImageService imageService)
        {
            _manager = manager;
            _imageService = imageService;
        }

        // Public
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetAll([FromQuery] ProductQueryDto query)
        {
            var result = await _manager.GetAllAsync(query);
            return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResult(result));
        }

        // Public
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById([FromRoute] int id)
        {
            var product = await _manager.GetByIdAsync(id);
            if (product is null)
                return NotFound(ApiResponse<ProductDto>.FailResult($"Product with id {id} was not found."));
            return Ok(ApiResponse<ProductDto>.SuccessResult(product));
        }

        // Admin only
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductDto dto)
        {
            var product = await _manager.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id },
                ApiResponse<ProductDto>.SuccessResult(product, "Product created successfully."));
        }

        // Admin only
        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update([FromRoute] int id, [FromBody] UpdateProductDto dto)
        {
            var product = await _manager.UpdateAsync(id, dto);
            if (product is null)
                return NotFound(ApiResponse<ProductDto>.FailResult($"Product with id {id} was not found."));
            return Ok(ApiResponse<ProductDto>.SuccessResult(product, "Product updated successfully."));
        }

        // Admin only
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromRoute] int id)
        {
            var deleted = await _manager.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<bool>.FailResult($"Product with id {id} was not found."));
            return Ok(ApiResponse<bool>.SuccessResult(true, "Product deleted successfully."));
        }

        // Admin only
        [HttpPost("{id:int}/image")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> SetImage([FromRoute] int id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<ProductDto>.FailResult("No file was uploaded."));

            try
            {
                var url = await _imageService.UploadAsync(file);
                var product = await _manager.SetImageAsync(id, url);
                if (product is null)
                    return NotFound(ApiResponse<ProductDto>.FailResult($"Product with id {id} was not found."));
                return Ok(ApiResponse<ProductDto>.SuccessResult(product, "Product image updated."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<ProductDto>.FailResult(ex.Message));
            }
        }
    }
}
