using Application.DTOs.Product;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

/// <summary>
/// Catálogo de produtos.
/// Consultas são públicas. Criação, edição e exclusão são restritas a Admin.
/// </summary>
[Route("api/products")]
public sealed class ProductsController : ApiControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) => _productService = productService;

    /// <summary>[Público] Lista todos os produtos com paginação.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>[Público] Lista produtos com estoque disponível.</summary>
    [HttpGet("in-stock")]
    public async Task<IActionResult> GetInStock(CancellationToken cancellationToken)
    {
        var result = await _productService.GetInStockAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>[Público] Retorna um produto pelo ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetByIdAsync(id, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Admin] Cadastra um novo produto.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductDto dto, CancellationToken cancellationToken)
    {
        var result = await _productService.CreateAsync(dto, cancellationToken);
        return RespondCreated(result);
    }

    /// <summary>[Admin] Atualiza um produto existente.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateProductDto dto, CancellationToken cancellationToken)
    {
        var result = await _productService.UpdateAsync(id, dto, cancellationToken);
        return Respond(result);
    }

    /// <summary>[Admin] Remove um produto.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteAsync(id, cancellationToken);
        return Respond(result);
    }
}
