using AutoMapper;
using Application.DTOs.Coupon;
using Core.Entities;

namespace Application.Mappings;

public sealed class CouponMappingProfile : Profile
{
    public CouponMappingProfile()
    {
        // Entidade → Response
        CreateMap<Coupon, CouponResponseDto>();

        // CreateDto → Entidade
        // IsActive e TimesUsed têm valores padrão definidos na entidade (true e 0)
        CreateMap<CreateCouponDto, Coupon>()
            .ForMember(dest => dest.Id,         opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,  opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt,  opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,   opt => opt.Ignore())
            .ForMember(dest => dest.TimesUsed,  opt => opt.Ignore())
            .ForMember(dest => dest.Carts,      opt => opt.Ignore());

        // UpdateDto → Entidade (usado com mapper.Map(dto, existingEntity))
        // Code, DiscountPercentage e TimesUsed não são atualizáveis
        CreateMap<UpdateCouponDto, Coupon>()
            .ForMember(dest => dest.Id,                  opt => opt.Ignore())
            .ForMember(dest => dest.Code,                opt => opt.Ignore())
            .ForMember(dest => dest.DiscountPercentage,  opt => opt.Ignore())
            .ForMember(dest => dest.TimesUsed,           opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,           opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt,           opt => opt.Ignore())
            .ForMember(dest => dest.Carts,               opt => opt.Ignore());
    }
}
