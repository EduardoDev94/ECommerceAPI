using AutoMapper;
using Application.DTOs.User;
using Core.Entities;

namespace Application.Mappings;

public sealed class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Entidade → Response (Role é enum → string via ToString())
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        // CreateDto → Entidade
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id,           opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,    opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt,    opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,     opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CartId,       opt => opt.Ignore())
            .ForMember(dest => dest.Cart,         opt => opt.Ignore())
            .ForMember(dest => dest.Orders,       opt => opt.Ignore());

        // UpdateDto → Entidade (Role e PasswordHash não são alterados por UpdateDto)
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id,           opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,    opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt,    opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,     opt => opt.Ignore())
            .ForMember(dest => dest.Role,         opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CartId,       opt => opt.Ignore())
            .ForMember(dest => dest.Cart,         opt => opt.Ignore())
            .ForMember(dest => dest.Orders,       opt => opt.Ignore());
    }
}
