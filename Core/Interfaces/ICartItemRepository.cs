namespace Core.Repositories;

using Core.Entities;

/// <summary>
/// Interface especializada para repositório de Itens de Carrinho
/// </summary>
public interface ICartItemRepository : IRepository<CartItem>
{
    /// <summary>
    /// Obtém todos os itens de um carrinho
    /// </summary>
    Task<IEnumerable<CartItem>> GetByCartIdAsync(Guid cartId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um item do carrinho com seu produto
    /// </summary>
    Task<CartItem?> GetWithProductAsync(Guid cartItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove todos os itens de um carrinho
    /// </summary>
    Task<bool> DeleteByCartIdAsync(Guid cartId, CancellationToken cancellationToken = default);
}
