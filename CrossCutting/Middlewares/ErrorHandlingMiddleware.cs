using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CrossCutting.Middlewares;

/// <summary>
/// Captura todas as exceções não tratadas da pipeline, loga com o CorrelationId
/// (já presente no LogContext via CorrelationIdMiddleware) e retorna uma resposta
/// JSON padronizada com o status HTTP adequado.
/// </summary>
public sealed class ErrorHandlingMiddleware : IMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // O CorrelationId já está no LogContext do Serilog via CorrelationIdMiddleware,
        // mas também o recuperamos para incluí-lo explicitamente no corpo da resposta
        var correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString()
                            ?? "N/A";

        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Resposta já iniciada — não é possível alterar status/body. CorrelationId: {CorrelationId}", correlationId);
            return;
        }

        context.Response.ContentType = "application/json";

        if (exception is ValidationException validationEx)
        {
            var errors = validationEx.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning(
                "Erro de validação. CorrelationId: {CorrelationId} | Erros: {@Errors}",
                correlationId, errors);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new ErrorResponse(
                Success: false,
                Message: "Um ou mais erros de validação ocorreram.",
                Errors: errors,
                CorrelationId: correlationId,
                Timestamp: DateTime.UtcNow
            ), JsonOptions);

            return;
        }

        var (statusCode, message) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
            _logger.LogError(exception,
                "Erro interno não tratado. CorrelationId: {CorrelationId} | {ExceptionType}: {ExceptionMessage}",
                correlationId, exception.GetType().Name, exception.Message);
        else
            _logger.LogWarning(
                "Erro de requisição. CorrelationId: {CorrelationId} | {StatusCode} | {ExceptionMessage}",
                correlationId, statusCode, exception.Message);

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new ErrorResponse(
            Success: false,
            Message: message,
            Errors: [],
            CorrelationId: correlationId,
            Timestamp: DateTime.UtcNow
        ), JsonOptions);
    }

    /// <summary>
    /// Mapeia o tipo da exceção para um status HTTP e mensagem amigável.
    /// Exceções de domínio específicas devem ser adicionadas aqui conforme necessário.
    /// </summary>
    private static (int StatusCode, string Message) MapException(Exception exception) =>
        exception switch
        {
            KeyNotFoundException  => (StatusCodes.Status404NotFound, exception.Message),
            ArgumentException     => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidOperationException => (StatusCodes.Status409Conflict, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Acesso não autorizado."),
            NotImplementedException => (StatusCodes.Status501NotImplemented, "Funcionalidade não implementada."),
            _                     => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno. Tente novamente ou contate o suporte.")
        };
}

/// <summary>Corpo da resposta de erro — espelha o contrato de ApiResponse para erros.</summary>
internal sealed record ErrorResponse(
    bool Success,
    string Message,
    IReadOnlyList<string> Errors,
    string CorrelationId,
    DateTime Timestamp);
