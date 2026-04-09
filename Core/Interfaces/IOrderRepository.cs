namespace Core.Repositories;

using Core.Entities;
using Core.Enums;

/// <summary>
/// Interface especializada para repositório de Pedidos
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Obtém todos os pedidos de um usuário
    /// </summary>
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um pedido com todos os seus itens
    /// </summary>
    Task<Order?> GetWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém pedidos por status
    /// </summary>
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todos os pedidos (uso exclusivo Admin)
    /// </summary>
    Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
}
