using Application.DTOs.Auth;
using Application.DTOs.Cart;
using Application.DTOs.Coupon;
using Application.DTOs.Order;
using Application.DTOs.Product;
using Application.DTOs.User;
using Bogus;
using Core.Enums;

namespace ECommerceAPI.UnitTests.Builders;

/// <summary>
/// Fábrica centralizada de DTOs usando Bogus.
/// Todos os métodos geram dados válidos por padrão.
/// </summary>
public static class DtoFakers
{
    public static RegisterDto ValidRegisterDto() =>
        new Faker<RegisterDto>()
            .RuleFor(r => r.Name,     f => f.Name.FullName())
            .RuleFor(r => r.Email,    f => f.Internet.Email())
            .RuleFor(r => r.Password, _ => "Password123!")
            .RuleFor(r => r.Role,     _ => UserRole.Customer)
            .Generate();

    public static LoginDto ValidLoginDto(string email, string password) =>
        new LoginDto { Email = email, Password = password };

    public static CreateProductDto ValidCreateProductDto() =>
        new Faker<CreateProductDto>()
            .RuleFor(p => p.Name,        f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Sentence(10))
            .RuleFor(p => p.Price,       f => Math.Round(f.Finance.Amount(10, 500), 2))
            .RuleFor(p => p.Stock,       f => f.Random.Int(1, 100))
            .Generate();

    public static UpdateProductDto ValidUpdateProductDto() =>
        new Faker<UpdateProductDto>()
            .RuleFor(p => p.Name,        f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Sentence(10))
            .RuleFor(p => p.Price,       f => Math.Round(f.Finance.Amount(10, 500), 2))
            .RuleFor(p => p.Stock,       f => f.Random.Int(1, 100))
            .Generate();

    public static CreateUserDto ValidCreateUserDto() =>
        new Faker<CreateUserDto>()
            .RuleFor(u => u.Name,  f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Role,  _ => UserRole.Customer)
            .Generate();

    public static UpdateUserDto ValidUpdateUserDto() =>
        new Faker<UpdateUserDto>()
            .RuleFor(u => u.Name,  f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .Generate();

    public static CreateCouponDto ValidCreateCouponDto() =>
        new Faker<CreateCouponDto>()
            .RuleFor(c => c.Code,               f => f.Random.AlphaNumeric(8).ToUpper())
            .RuleFor(c => c.DiscountPercentage, f => Math.Round(f.Random.Decimal(5, 50), 2))
            .RuleFor(c => c.ExpirationDate,     f => f.Date.Future(1).ToUniversalTime())
            .RuleFor(c => c.UsageLimit,         f => f.Random.Int(10, 100))
            .Generate();

    public static UpdateCouponDto ValidUpdateCouponDto() =>
        new Faker<UpdateCouponDto>()
            .RuleFor(c => c.ExpirationDate, f => f.Date.Future(1).ToUniversalTime())
            .RuleFor(c => c.UsageLimit,     f => f.Random.Int(10, 100))
            .Generate();

    public static AddCartItemDto ValidAddCartItemDto(Guid? productId = null) =>
        new Faker<AddCartItemDto>()
            .RuleFor(a => a.ProductId, _ => productId ?? Guid.NewGuid())
            .RuleFor(a => a.Quantity,  f => f.Random.Int(1, 5))
            .Generate();

    public static UpdateCartItemDto ValidUpdateCartItemDto() =>
        new Faker<UpdateCartItemDto>()
            .RuleFor(u => u.Quantity, f => f.Random.Int(1, 10))
            .Generate();

    public static ApplyCouponDto ValidApplyCouponDto(string? code = null) =>
        new ApplyCouponDto { Code = code ?? new Faker().Random.AlphaNumeric(8).ToUpper() };

    public static UpdateOrderStatusDto ValidUpdateOrderStatusDto(string status = "Paid") =>
        new UpdateOrderStatusDto { Status = status };
}
