namespace Core.Entities;

using Core.Common;
using Core.Enums;

/// <summary>
/// Entidade de Pedido
/// Representa um pedido finalizado (histórico de compra)
/// 
/// ⚠️ CRÍTICO: Order é IMUTÁVEL após criação
/// Os dados do pedido não devem ser alterados após a criação
/// 
/// Relacionamentos:
/// - N:1 com User
/// - 1:N com OrderItems
/// </summary>
public class Order : Entity
{
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? CouponCode { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Relacionamentos
    public virtual User? User { get; set; }
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    /// <summary>
    /// Atualiza o status do pedido
    /// É a única propriedade que pode ser alterada após criação
    /// </summary>
    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}
