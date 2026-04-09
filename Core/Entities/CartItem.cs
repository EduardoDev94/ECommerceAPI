namespace Core.Entities;

using Core.Common;

/// <summary>
/// Entidade de Item do Carrinho
/// Representa um item dentro do carrinho de compras
/// 
/// Relacionamentos:
/// - N:1 com Cart
/// - N:1 com Product
/// </summary>
public class CartItem : Entity
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    // Relacionamentos
    public virtual Cart? Cart { get; set; }
    public virtual Product? Product { get; set; }

    /// <summary>
    /// Calcula o preço total do item (quantidade × preço unitário)
    /// </summary>
    public decimal TotalPrice => Quantity * (Product?.Price ?? 0);
}
