using Core.Repositories;
using Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository,      UserRepository>();
        services.AddScoped<IProductRepository,   ProductRepository>();
        services.AddScoped<ICartRepository,      CartRepository>();
        services.AddScoped<ICartItemRepository,  CartItemRepository>();
        services.AddScoped<ICouponRepository,    CouponRepository>();
        services.AddScoped<IOrderRepository,     OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();

        return services;
    }
}
