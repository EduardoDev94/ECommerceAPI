using AutoMapper;
using Application.DTOs.Product;
using Core.Entities;

namespace Application.Mappings;

public sealed class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Entidade → Response
        CreateMap<Product, ProductResponseDto>();

        // CreateDto → Entidade
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id,         opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,  opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt,  opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,   opt => opt.Ignore())
            .ForMember(dest => dest.CartItems,  opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        // UpdateDto → Entidade (usado com mapper.Map(dto, existingEntity))
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id,         opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,  opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt,  opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,   opt => opt.Ignore())
            .ForMember(dest => dest.CartItems,  opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
    }
}
