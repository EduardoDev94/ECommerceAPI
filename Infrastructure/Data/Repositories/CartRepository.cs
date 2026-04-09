using Core.Entities;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public sealed class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(ECommerceDbContext context) : base(context) { }

    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public async Task<Cart?> GetWithItemsAsync(Guid cartId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);

    public async Task<bool> ClearAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        var cart = await GetWithItemsAsync(cartId, cancellationToken);
        if (cart is null) return false;

        cart.Clear();
        await UpdateAsync(cart, cancellationToken);
        return true;
    }
}
