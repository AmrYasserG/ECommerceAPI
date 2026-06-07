namespace ECommerceAPI.BLL.DTOs
{
    public class ProductQueryDto
    {
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
