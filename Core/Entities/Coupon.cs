namespace Core.Entities;

using Core.Common;

/// <summary>
/// Entidade de Cupom de Desconto
/// Representa um cupom de desconto com regras de expiração e limite de uso
/// 
/// Relacionamentos:
/// - 1:N com Carts (opcional)
/// </summary>
public class Coupon : Entity
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int UsageLimit { get; set; }
    public int TimesUsed { get; set; } = 0;

    // Relacionamentos
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    /// <summary>
    /// Valida se o cupom pode ser utilizado
    /// </summary>
    public bool IsValid()
    {
        return IsActive 
            && DateTime.UtcNow <= ExpirationDate 
            && TimesUsed < UsageLimit;
    }

    /// <summary>
    /// Incrementa o contador de uso do cupom
    /// </summary>
    public void IncrementUsage()
    {
        if (TimesUsed < UsageLimit)
        {
            TimesUsed++;
        }
    }
}
