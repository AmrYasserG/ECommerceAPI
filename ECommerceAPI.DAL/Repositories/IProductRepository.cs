using ECommerceAPI.Common.Models;

namespace ECommerceAPI.DAL.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        Task<Product?> GetByIdWithCategoryAsync(int id);
        Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredAsync(
            int? categoryId, string? name, int pageNumber, int pageSize);
    }
}
