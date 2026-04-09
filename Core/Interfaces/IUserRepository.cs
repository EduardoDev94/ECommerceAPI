namespace Core.Repositories;

using Core.Entities;

/// <summary>
/// Interface especializada para repositório de Usuários
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Obtém um usuário pelo email
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário com seu carrinho
    /// </summary>
    Task<User?> GetWithCartAsync(Guid userId, CancellationToken cancellationToken = default);
}
