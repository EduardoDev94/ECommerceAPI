using AutoMapper;
using Application.DTOs.Cart;
using Core.Entities;

namespace Application.Mappings;

public sealed class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        // CartItem → CartItemResponseDto
        // ProductName e ProductPrice vêm da navegação Product (requer Include no repositório)
        CreateMap<CartItem, CartItemResponseDto>()
            .ForMember(dest => dest.ProductName,  opt => opt.MapFrom(src => src.Product!.Name))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product!.Price))
            .ForMember(dest => dest.TotalPrice,   opt => opt.Ignore()); // propriedade computada

        // Cart → CartResponseDto
        // CouponCode vem da navegação Coupon (requer Include no repositório)
        CreateMap<Cart, CartResponseDto>()
            .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null));

        // AddCartItemDto → CartItem
        CreateMap<AddCartItemDto, CartItem>()
            .ForMember(dest => dest.Id,        opt => opt.Ignore())
            .ForMember(dest => dest.CartId,    opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,  opt => opt.Ignore())
            .ForMember(dest => dest.Cart,      opt => opt.Ignore())
            .ForMember(dest => dest.Product,   opt => opt.Ignore());

        // UpdateCartItemDto → CartItem (apenas Quantity é atualizado)
        CreateMap<UpdateCartItemDto, CartItem>()
            .ForMember(dest => dest.Id,        opt => opt.Ignore())
            .ForMember(dest => dest.CartId,    opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,  opt => opt.Ignore())
            .ForMember(dest => dest.Cart,      opt => opt.Ignore())
            .ForMember(dest => dest.Product,   opt => opt.Ignore());
    }
}
