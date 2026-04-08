using Application.DTOs.Order;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

/// <summary>
/// Gerenciamento de pedidos.
/// Clientes consultam os próprios pedidos. Admin tem visão global e pode alterar status.
/// </summary>
[Route("api/orders")]
[Authorize]
public sealed class OrdersController : ApiControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    /// <summary>[Autenticado] Lista os pedidos do usuário logado.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByUserIdAsync(CurrentUserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>[Admin] Lista todos os pedidos, com filtro opcional por status.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(status))
        {
            var byStatus = await _orderService.GetByStatusAsync(status, cancellationToken);
            return Ok(byStatus);
        }

        var all = await _orderService.GetAllAsync(cancellationToken);
        return Ok(all);
    }

    /// <summary>[Admin] Retorna um pedido pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(id, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Admin] Atualiza o status de um pedido (ex.: Paid, Shipped, Delivered, Cancelled).</summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateOrderStatusDto dto, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateStatusAsync(id, dto, cancellationToken);
        return Respond(result);
    }
}
