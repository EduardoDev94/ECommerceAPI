using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CrossCutting.Middlewares;

/// <summary>
/// Garante que cada requisição possua um Correlation-ID único.
/// Fluxo:
///   1. Lê X-Correlation-Id do header de entrada (propagação de sistemas externos)
///   2. Se ausente, gera um novo GUID
///   3. Armazena em HttpContext.Items para uso interno
///   4. Injeta no LogContext do Serilog — todos os logs da requisição carregarão {CorrelationId}
///   5. Devolve o ID no header de resposta X-Correlation-Id
/// </summary>
public sealed class CorrelationIdMiddleware : IMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = ResolveCorrelationId(context);

        // Disponível para outros middlewares e controllers via HttpContext.Items
        context.Items[HeaderName] = correlationId;

        // OnStarting garante que o header seja adicionado mesmo quando a resposta
        // começa a ser escrita antes do middleware retornar
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // Enriquece o LogContext do Serilog: todos os logs dentro deste using
        // terão a propriedade {CorrelationId} automaticamente
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.LogDebug("Requisição recebida [{Method}] {Path} | CorrelationId: {CorrelationId}",
                context.Request.Method, context.Request.Path, correlationId);

            await next(context);

            _logger.LogDebug("Requisição finalizada [{Method}] {Path} | Status: {StatusCode} | CorrelationId: {CorrelationId}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, correlationId);
        }
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        var fromHeader = context.Request.Headers[HeaderName].FirstOrDefault();

        return string.IsNullOrWhiteSpace(fromHeader)
            ? Guid.NewGuid().ToString()
            : fromHeader;
    }
}
