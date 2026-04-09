using Application.DTOs.Cart;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

/// <summary>
/// Operações do carrinho do usuário autenticado.
/// O carrinho é sempre identificado pelo token JWT — não há userId na rota.
/// </summary>
[Route("api/cart")]
[Authorize]
public sealed class CartController : ApiControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) => _cartService = cartService;

    /// <summary>[Autenticado] Retorna o carrinho do usuário logado.</summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _cartService.GetByUserIdAsync(CurrentUserId, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Autenticado] Adiciona um produto ao carrinho.</summary>
    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddCartItemDto dto, CancellationToken cancellationToken)
    {
        var result = await _cartService.AddItemAsync(CurrentUserId, dto, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Autenticado] Atualiza a quantidade de um item do carrinho.</summary>
    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid itemId, UpdateCartItemDto dto, CancellationToken cancellationToken)
    {
        var result = await _cartService.UpdateItemAsync(CurrentUserId, itemId, dto, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Autenticado] Remove um item do carrinho.</summary>
    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await _cartService.RemoveItemAsync(CurrentUserId, itemId, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Autenticado] Aplica um cupom de desconto ao carrinho.</summary>
    [HttpPost("coupon")]
    public async Task<IActionResult> ApplyCoupon(ApplyCouponDto dto, CancellationToken cancellationToken)
    {
        var result = await _cartService.ApplyCouponAsync(CurrentUserId, dto, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Autenticado] Remove o cupom aplicado ao carrinho.</summary>
    [HttpDelete("coupon")]
    public async Task<IActionResult> RemoveCoupon(CancellationToken cancellationToken)
    {
        var result = await _cartService.RemoveCouponAsync(CurrentUserId, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Autenticado] Finaliza a compra e gera um pedido a partir do carrinho.</summary>
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
    {
        var result = await _cartService.CheckoutAsync(CurrentUserId, cancellationToken);
        return RespondCreated(result);
    }
}
