namespace Core.Entities;

using Core.Common;

/// <summary>
/// Entidade de Carrinho
/// Representa o carrinho ativo do usuário onde produtos são adicionados antes da compra
/// 
/// Relacionamentos:
/// - N:1 com User
/// - 1:N com CartItems
/// - 0..1 com Coupon
/// </summary>
public class Cart : Entity
{
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal FinalAmount { get; set; } = 0;
    public Guid? CouponId { get; set; }

    // Relacionamentos
    public virtual User? User { get; set; }
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    public virtual Coupon? Coupon { get; set; }

    /// <summary>
    /// Calcula o total do carrinho baseado nos itens
    /// </summary>
    public void CalculateTotals()
    {
        TotalAmount = Items.Sum(item => item.Quantity * item.Product!.Price);
        
        if (Coupon != null && Coupon.IsValid())
        {
            DiscountAmount = TotalAmount * (Coupon.DiscountPercentage / 100);
        }
        else
        {
            DiscountAmount = 0;
            CouponId = null;
        }

        FinalAmount = TotalAmount - DiscountAmount;
    }

    /// <summary>
    /// Adiciona um item ao carrinho
    /// </summary>
    public void AddItem(CartItem item)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            Items.Add(item);
        }

        CalculateTotals();
    }

    /// <summary>
    /// Remove um item do carrinho
    /// </summary>
    public void RemoveItem(Guid cartItemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item != null)
        {
            Items.Remove(item);
            CalculateTotals();
        }
    }

    /// <summary>
    /// Limpa todos os itens do carrinho
    /// </summary>
    public void Clear()
    {
        Items.Clear();
        CouponId = null;
        TotalAmount = 0;
        DiscountAmount = 0;
        FinalAmount = 0;
    }
}
