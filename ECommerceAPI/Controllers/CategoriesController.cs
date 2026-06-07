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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryManager _manager;
        private readonly IImageService _imageService;

        public CategoriesController(ICategoryManager manager, IImageService imageService)
        {
            _manager = manager;
            _imageService = imageService;
        }

        // Public
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAll()
        {
            var categories = await _manager.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
        }

        // Public
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById([FromRoute] int id)
        {
            var category = await _manager.GetByIdAsync(id);
            if (category is null)
                return NotFound(ApiResponse<CategoryDto>.FailResult($"Category with id {id} was not found."));
            return Ok(ApiResponse<CategoryDto>.SuccessResult(category));
        }

        // Admin only
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryDto dto)
        {
            var category = await _manager.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id },
                ApiResponse<CategoryDto>.SuccessResult(category, "Category created successfully."));
        }

        // Admin only
        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Update([FromRoute] int id, [FromBody] UpdateCategoryDto dto)
        {
            var category = await _manager.UpdateAsync(id, dto);
            if (category is null)
                return NotFound(ApiResponse<CategoryDto>.FailResult($"Category with id {id} was not found."));
            return Ok(ApiResponse<CategoryDto>.SuccessResult(category, "Category updated successfully."));
        }

        // Admin only
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromRoute] int id)
        {
            var deleted = await _manager.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<bool>.FailResult($"Category with id {id} was not found."));
            return Ok(ApiResponse<bool>.SuccessResult(true, "Category deleted successfully."));
        }

        // Admin only
        [HttpPost("{id:int}/image")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> SetImage([FromRoute] int id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<CategoryDto>.FailResult("No file was uploaded."));

            try
            {
                var url = await _imageService.UploadAsync(file);
                var category = await _manager.SetImageAsync(id, url);
                if (category is null)
                    return NotFound(ApiResponse<CategoryDto>.FailResult($"Category with id {id} was not found."));
                return Ok(ApiResponse<CategoryDto>.SuccessResult(category, "Category image updated."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CategoryDto>.FailResult(ex.Message));
            }
        }
    }
}
