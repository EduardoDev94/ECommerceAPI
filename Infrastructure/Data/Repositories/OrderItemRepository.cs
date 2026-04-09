using Core.Entities;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public sealed class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(ECommerceDbContext context) : base(context) { }

    public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync(cancellationToken);

    public async Task<OrderItem?> GetWithProductAsync(Guid orderItemId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(oi => oi.Product)
            .FirstOrDefaultAsync(oi => oi.Id == orderItemId, cancellationToken);
}
