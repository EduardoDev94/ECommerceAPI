namespace Core.Repositories;

using Core.Entities;

/// <summary>
/// Interface especializada para repositório de Cupons
/// </summary>
public interface ICouponRepository : IRepository<Coupon>
{
    /// <summary>
    /// Obtém um cupom pelo código
    /// </summary>
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém cupons ativos e não expirados
    /// </summary>
    Task<IEnumerable<Coupon>> GetActiveCouponsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida e aplica um cupom (verifica disponibilidade)
    /// </summary>
    Task<Coupon?> ValidateAndApplyAsync(string code, CancellationToken cancellationToken = default);
}
