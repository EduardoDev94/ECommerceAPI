using Application.DTOs.Coupon;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

/// <summary>
/// Gerenciamento de cupons de desconto.
/// Todos os endpoints são restritos a Admin.
/// </summary>
[Route("api/coupons")]
[Authorize(Roles = "Admin")]
public sealed class CouponsController : ApiControllerBase
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService) => _couponService = couponService;

    /// <summary>[Admin] Lista todos os cupons.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _couponService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>[Admin] Lista apenas cupons ativos e dentro da validade.</summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var result = await _couponService.GetActiveAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>[Admin] Retorna um cupom pelo ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _couponService.GetByIdAsync(id, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Admin] Cria um novo cupom de desconto.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateCouponDto dto, CancellationToken cancellationToken)
    {
        var result = await _couponService.CreateAsync(dto, cancellationToken);
        return RespondCreated(result);
    }

    /// <summary>[Admin] Atualiza validade e limite de uso de um cupom.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCouponDto dto, CancellationToken cancellationToken)
    {
        var result = await _couponService.UpdateAsync(id, dto, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Admin] Remove um cupom.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _couponService.DeleteAsync(id, cancellationToken);
        return Respond(result);
    }
}
