using AutoMapper;
using Application.DTOs.Order;
using Core.Entities;

namespace Application.Mappings;

public sealed class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // OrderItem → OrderItemResponseDto
        // ProductName vem da navegação Product (requer Include no repositório)
        // UnitPrice já está armazenado no OrderItem (valor histórico no momento da compra)
        CreateMap<OrderItem, OrderItemResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
            .ForMember(dest => dest.TotalPrice,  opt => opt.Ignore()); // propriedade computada

        // Order → OrderResponseDto
        // Status (enum OrderStatus) é convertido para string legível
        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
