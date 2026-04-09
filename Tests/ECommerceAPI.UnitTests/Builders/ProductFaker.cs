using Bogus;
using Core.Entities;

namespace ECommerceAPI.UnitTests.Builders;

/// <summary>Faker para a entidade Product com dados realistas via Bogus.</summary>
public sealed class ProductFaker : Faker<Product>
{
    public ProductFaker()
    {
        RuleFor(p => p.Id,          f => f.Random.Guid());
        RuleFor(p => p.Name,        f => f.Commerce.ProductName());
        RuleFor(p => p.Description, f => f.Lorem.Sentence(10));
        RuleFor(p => p.Price,       f => Math.Round(f.Finance.Amount(10, 1000), 2));
        RuleFor(p => p.Stock,       f => f.Random.Int(1, 100));
        RuleFor(p => p.IsActive,    _ => true);
        RuleFor(p => p.CreatedAt,   f => f.Date.Past(1).ToUniversalTime());
        RuleFor(p => p.UpdatedAt,   f => f.Date.Recent(30).ToUniversalTime());
    }
}
