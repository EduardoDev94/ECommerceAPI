using Core.Entities;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public sealed class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ECommerceDbContext context) : base(context) { }

    public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);

    public async Task<IEnumerable<Product>> GetInStockAsync(CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .Where(p => p.Stock > 0 && p.IsActive)
            .ToListAsync(cancellationToken);

    public async Task<bool> ReduceStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(productId, cancellationToken);
        if (product is null || product.Stock < quantity) return false;

        product.Stock -= quantity;
        await UpdateAsync(product, cancellationToken);
        return true;
    }
}
