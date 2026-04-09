namespace Core.Repositories;

using Core.Entities;

/// <summary>
/// Interface especializada para repositório de Produtos
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Obtém um produto pelo nome
    /// </summary>
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos em estoque
    /// </summary>
    Task<IEnumerable<Product>> GetInStockAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reduz o estoque de um produto
    /// </summary>
    Task<bool> ReduceStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}
