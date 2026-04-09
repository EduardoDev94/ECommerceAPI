namespace Core.Entities;

using Core.Common;

/// <summary>
/// Entidade de Produto
/// Representa um produto disponível no catálogo
/// 
/// Observação: O preço pode mudar ao longo do tempo
/// </summary>
public class Product : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    // Relacionamentos
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
