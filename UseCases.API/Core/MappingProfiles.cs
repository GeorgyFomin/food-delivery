using AutoMapper;
using Entities.Domain;
using UseCases.API.Dto;

namespace UseCases.API.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<MenuItem, MenuItemDto>();
            CreateMap<Menu, MenuDto>();
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<Order, OrderDto>();
            CreateMap<Discount, DiscountDto>();
            CreateMap<ProductIngredient, ProductIngredientDto>();
            CreateMap<Ingredient, IngredientDto>();
            CreateMap<Delivery, DeliveryDto>();
            CreateMap<Product, ProductDto>();
        }
    }
}
