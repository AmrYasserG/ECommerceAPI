using ECommerceAPI.BLL.DTOs;

namespace ECommerceAPI.BLL.Managers
{
    public interface IProductManager
    {
        Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryDto query);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
        Task<ProductDto?> SetImageAsync(int id, string imageUrl);
    }
}
