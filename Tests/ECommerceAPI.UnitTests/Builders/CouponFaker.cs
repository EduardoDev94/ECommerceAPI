using Bogus;
using Core.Entities;

namespace ECommerceAPI.UnitTests.Builders;

/// <summary>Faker para a entidade Coupon com dados válidos via Bogus.</summary>
public sealed class CouponFaker : Faker<Coupon>
{
    public CouponFaker()
    {
        RuleFor(c => c.Id,                 f => f.Random.Guid());
        RuleFor(c => c.Code,               f => f.Random.AlphaNumeric(8).ToUpper());
        RuleFor(c => c.DiscountPercentage, f => Math.Round(f.Random.Decimal(5, 50), 2));
        RuleFor(c => c.ExpirationDate,     f => f.Date.Future(1).ToUniversalTime());
        RuleFor(c => c.IsActive,           _ => true);
        RuleFor(c => c.UsageLimit,         f => f.Random.Int(10, 100));
        RuleFor(c => c.TimesUsed,          _ => 0);
        RuleFor(c => c.CreatedAt,          f => f.Date.Past(1).ToUniversalTime());
        RuleFor(c => c.UpdatedAt,          f => f.Date.Recent(30).ToUniversalTime());
    }

    /// <summary>Gera um cupom expirado (inválido por data).</summary>
    public Coupon GenerateExpired() =>
        this.RuleFor(c => c.ExpirationDate, f => f.Date.Past(1).ToUniversalTime()).Generate();

    /// <summary>Gera um cupom no limite máximo de uso (inválido por uso).</summary>
    public Coupon GenerateExhausted()
    {
        var coupon = Generate();
        coupon.TimesUsed = coupon.UsageLimit;
        return coupon;
    }
}
