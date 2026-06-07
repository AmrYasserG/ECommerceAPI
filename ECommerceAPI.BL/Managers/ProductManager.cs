using AutoMapper;
using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.Common.Models;
using ECommerceAPI.DAL.UnitOfWork;

namespace ECommerceAPI.BLL.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProductManager(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryDto query)
        {
            query.PageSize = Math.Min(query.PageSize, 50);
            query.PageNumber = Math.Max(query.PageNumber, 1);

            var (products, totalCount) = await _uow.Products.GetFilteredAsync(
                query.CategoryId, query.Name, query.PageNumber, query.PageSize);

            return new PagedResult<ProductDto>
            {
                Data = _mapper.Map<IEnumerable<ProductDto>>(products),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _uow.Products.GetByIdWithCategoryAsync(id);
            return product is null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _uow.Products.AddAsync(product);
            await _uow.SaveChangesAsync();
            var created = await _uow.Products.GetByIdWithCategoryAsync(product.Id);
            return _mapper.Map<ProductDto>(created!);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _uow.Products.GetByIdWithCategoryAsync(id);
            if (product is null) return null;
            _mapper.Map(dto, product);
            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();
            var updated = await _uow.Products.GetByIdWithCategoryAsync(id);
            return _mapper.Map<ProductDto>(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product is null) return false;
            _uow.Products.Delete(product);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<ProductDto?> SetImageAsync(int id, string imageUrl)
        {
            var product = await _uow.Products.GetByIdWithCategoryAsync(id);
            if (product is null) return null;
            product.ImageUrl = imageUrl;
            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }
    }
}
