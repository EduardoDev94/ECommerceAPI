using Bogus;
using Core.Entities;

namespace ECommerceAPI.UnitTests.Builders;

/// <summary>Faker para a entidade Cart via Bogus.</summary>
public sealed class CartFaker : Faker<Cart>
{
    public CartFaker()
    {
        RuleFor(c => c.Id,             f => f.Random.Guid());
        RuleFor(c => c.UserId,         f => f.Random.Guid());
        RuleFor(c => c.TotalAmount,    _ => 0m);
        RuleFor(c => c.DiscountAmount, _ => 0m);
        RuleFor(c => c.FinalAmount,    _ => 0m);
        RuleFor(c => c.IsActive,       _ => true);
        RuleFor(c => c.CreatedAt,      f => f.Date.Past(1).ToUniversalTime());
        RuleFor(c => c.UpdatedAt,      f => f.Date.Recent(30).ToUniversalTime());
    }

    /// <summary>Gera um carrinho com itens preenchidos (Product carregado).</summary>
    public Cart GenerateWithItems(Product product, int quantity = 2)
    {
        var cart = Generate();
        var item = new CartItem
        {
            Id        = Guid.NewGuid(),
            CartId    = cart.Id,
            ProductId = product.Id,
            Product   = product,
            Quantity  = quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive  = true
        };
        cart.Items.Add(item);
        cart.TotalAmount  = product.Price * quantity;
        cart.FinalAmount  = cart.TotalAmount;
        return cart;
    }
}
