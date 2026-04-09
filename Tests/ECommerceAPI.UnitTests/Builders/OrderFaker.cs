using Bogus;
using Core.Entities;
using Core.Enums;

namespace ECommerceAPI.UnitTests.Builders;

/// <summary>Faker para a entidade Order via Bogus.</summary>
public sealed class OrderFaker : Faker<Order>
{
    public OrderFaker()
    {
        RuleFor(o => o.Id,             f => f.Random.Guid());
        RuleFor(o => o.UserId,         f => f.Random.Guid());
        RuleFor(o => o.OrderDate,      f => f.Date.Past(1).ToUniversalTime());
        RuleFor(o => o.TotalAmount,    f => Math.Round(f.Finance.Amount(10, 1000), 2));
        RuleFor(o => o.DiscountAmount, _ => 0m);
        RuleFor(o => o.FinalAmount,    f => Math.Round(f.Finance.Amount(10, 1000), 2));
        RuleFor(o => o.Status,         f => f.PickRandom<OrderStatus>());
        RuleFor(o => o.IsActive,       _ => true);
        RuleFor(o => o.CreatedAt,      f => f.Date.Past(1).ToUniversalTime());
        RuleFor(o => o.UpdatedAt,      f => f.Date.Recent(30).ToUniversalTime());
    }
}
