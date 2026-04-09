namespace Core.Repositories;

using Core.Entities;

/// <summary>
/// Interface especializada para repositório de Carrinhos
/// </summary>
public interface ICartRepository : IRepository<Cart>
{
    /// <summary>
    /// Obtém o carrinho ativo de um usuário
    /// </summary>
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém carrinho com todos os seus itens e dados relacionados
    /// </summary>
    Task<Cart?> GetWithItemsAsync(Guid cartId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Limpa um carrinho (remove todos os itens)
    /// </summary>
    Task<bool> ClearAsync(Guid cartId, CancellationToken cancellationToken = default);
}
