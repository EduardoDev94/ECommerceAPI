namespace Core.Entities;

using Core.Common;

/// <summary>
/// Entidade de Item do Pedido
/// Representa um item dentro de um pedido finalizado
/// 
/// ⚠️ CRÍTICO: UnitPrice deve armazenar o valor do produto no momento da compra
/// Isso garante consistência histórica, mesmo que o preço do produto mude depois
/// 
/// Relacionamentos:
/// - N:1 com Order
/// - N:1 com Product
/// </summary>
public class OrderItem : Entity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Relacionamentos
    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }

    /// <summary>
    /// Calcula o preço total do item no pedido
    /// </summary>
    public decimal TotalPrice => Quantity * UnitPrice;

    /// <summary>
    /// Cria um OrderItem a partir de um CartItem
    /// Captura o preço atual do produto para manter histórico
    /// </summary>
    public static OrderItem CreateFromCartItem(CartItem cartItem)
    {
        if (cartItem.Product == null)
            throw new InvalidOperationException("Product must be loaded");

        return new OrderItem
        {
            OrderId = Guid.Empty, // Será preenchido ao associar ao Order
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity,
            UnitPrice = cartItem.Product.Price
        };
    }
}
