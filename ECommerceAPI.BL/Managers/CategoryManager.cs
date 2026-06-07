using AutoMapper;
using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.Common.Models;
using ECommerceAPI.DAL.UnitOfWork;

namespace ECommerceAPI.BLL.Managers
{
    public class CategoryManager : ICategoryManager
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryManager(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _uow.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            return category is null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _uow.Categories.AddAsync(category);
            await _uow.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category is null) return null;
            _mapper.Map(dto, category);
            _uow.Categories.Update(category);
            await _uow.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category is null) return false;
            _uow.Categories.Delete(category);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<CategoryDto?> SetImageAsync(int id, string imageUrl)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category is null) return null;
            category.ImageUrl = imageUrl;
            _uow.Categories.Update(category);
            await _uow.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
