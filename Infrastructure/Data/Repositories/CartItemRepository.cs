using Core.Entities;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public sealed class CartItemRepository : BaseRepository<CartItem>, ICartItemRepository
{
    public CartItemRepository(ECommerceDbContext context) : base(context) { }

    public async Task<IEnumerable<CartItem>> GetByCartIdAsync(Guid cartId, CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .Where(ci => ci.CartId == cartId)
            .ToListAsync(cancellationToken);

    public async Task<CartItem?> GetWithProductAsync(Guid cartItemId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId, cancellationToken);

    public async Task<bool> DeleteByCartIdAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        var items = await _dbSet
            .Where(ci => ci.CartId == cartId)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(items);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
