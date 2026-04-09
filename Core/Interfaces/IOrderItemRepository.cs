namespace Core.Repositories;

using Core.Entities;

/// <summary>
/// Interface especializada para repositório de Itens de Pedido
/// </summary>
public interface IOrderItemRepository : IRepository<OrderItem>
{
    /// <summary>
    /// Obtém todos os itens de um pedido
    /// </summary>
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um item do pedido com seu produto
    /// </summary>
    Task<OrderItem?> GetWithProductAsync(Guid orderItemId, CancellationToken cancellationToken = default);
}
