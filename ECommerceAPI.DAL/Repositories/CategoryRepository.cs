using ECommerceAPI.Common.Models;
using ECommerceAPI.DAL.Context;

namespace ECommerceAPI.DAL.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }
    }
}
