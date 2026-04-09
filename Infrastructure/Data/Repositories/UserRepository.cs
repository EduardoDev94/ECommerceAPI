using Core.Entities;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ECommerceDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetWithCartAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(u => u.Cart)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
}
