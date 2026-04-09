using Application.Common;
using Bogus;
using Core.Entities;
using Core.Enums;

namespace ECommerceAPI.UnitTests.Builders;

/// <summary>Faker para a entidade User com dados realistas via Bogus.</summary>
public sealed class UserFaker : Faker<User>
{
    public UserFaker()
    {
        RuleFor(u => u.Id,           f => f.Random.Guid());
        RuleFor(u => u.Name,         f => f.Name.FullName());
        RuleFor(u => u.Email,        f => f.Internet.Email());
        RuleFor(u => u.PasswordHash, _ => PasswordHasher.Hash("Password123!"));
        RuleFor(u => u.Role,         f => f.PickRandom<UserRole>());
        RuleFor(u => u.IsActive,     _ => true);
        RuleFor(u => u.CreatedAt,    f => f.Date.Past(1).ToUniversalTime());
        RuleFor(u => u.UpdatedAt,    f => f.Date.Recent(30).ToUniversalTime());
    }
}
