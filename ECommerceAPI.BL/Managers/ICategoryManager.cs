using ECommerceAPI.BLL.DTOs;

namespace ECommerceAPI.BLL.Managers
{
    public interface ICategoryManager
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteAsync(int id);
        Task<CategoryDto?> SetImageAsync(int id, string imageUrl);
    }
}
