using Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Extrai o ID do usuário autenticado a partir do claim 'sub' do JWT.</summary>
    protected Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id)
                ? id
                : throw new UnauthorizedAccessException("Token inválido ou ausente.");
        }
    }

    /// <summary>
    /// Converte um ApiResponse em IActionResult com status HTTP adequado.
    /// Detecta "não encontrado" automaticamente para retornar 404.
    /// </summary>
    protected IActionResult Respond<T>(ApiResponse<T> result)
    {
        if (result.Success) return Ok(result);

        if (result.Message.Contains("não encontrado", StringComparison.OrdinalIgnoreCase))
            return NotFound(result);

        return BadRequest(result);
    }

    /// <summary>Respond com 201 Created ao invés de 200 OK em caso de sucesso.</summary>
    protected IActionResult RespondCreated<T>(ApiResponse<T> result)
    {
        if (result.Success) return StatusCode(StatusCodes.Status201Created, result);
        return BadRequest(result);
    }
}
