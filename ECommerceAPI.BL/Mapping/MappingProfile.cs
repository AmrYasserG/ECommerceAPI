using AutoMapper;
using ECommerceAPI.BLL.DTOs;
using ECommerceAPI.Common.Models;

namespace ECommerceAPI.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            // Product
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Cart
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName,     opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.UnitPrice,       opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.TotalPrice,      opt => opt.MapFrom(src => src.Quantity * src.Product.Price));

            // Order
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.TotalPrice,  opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        }
    }
}
