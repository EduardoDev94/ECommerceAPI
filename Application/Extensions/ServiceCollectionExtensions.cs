using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos os serviços da camada Application:
    /// validadores FluentValidation, profiles AutoMapper e domain services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(config => config.AddMaps(assembly));

        services.AddScoped<IAuthService,    AuthService>();
        services.AddScoped<IUserService,    UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICouponService,  CouponService>();
        services.AddScoped<IOrderService,   OrderService>();
        services.AddScoped<ICartService,    CartService>();

        return services;
    }
}
