using Core.Entities;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public sealed class CouponRepository : BaseRepository<Coupon>, ICouponRepository
{
    public CouponRepository(ECommerceDbContext context) : base(context) { }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

    public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync(CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .Where(c => c.IsActive && c.ExpirationDate > DateTime.UtcNow && c.TimesUsed < c.UsageLimit)
            .ToListAsync(cancellationToken);

    public async Task<Coupon?> ValidateAndApplyAsync(string code, CancellationToken cancellationToken = default)
    {
        var coupon = await GetByCodeAsync(code, cancellationToken);
        if (coupon is null || !coupon.IsValid()) return null;

        coupon.IncrementUsage();
        await UpdateAsync(coupon, cancellationToken);
        return coupon;
    }
}
