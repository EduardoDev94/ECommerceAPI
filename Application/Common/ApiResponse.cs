namespace Application.Common;

/// <summary>
/// Envelope padrão para todas as respostas da API.
/// Use os métodos estáticos Ok() e Fail() para construir instâncias — 
/// o construtor privado impede criação arbitrária fora desta classe.
/// </summary>
public sealed class ApiResponse<T>
{
    /// <summary>Indica se a operação foi concluída com sucesso.</summary>
    public bool Success { get; private init; }

    /// <summary>Mensagem descritiva do resultado.</summary>
    public string Message { get; private init; } = string.Empty;

    /// <summary>Payload da resposta. Nulo em caso de falha.</summary>
    public T? Data { get; private init; }

    /// <summary>Lista de erros de validação ou de negócio.</summary>
    public IReadOnlyList<string> Errors { get; private init; } = [];

    /// <summary>Momento UTC em que a resposta foi gerada.</summary>
    public DateTime Timestamp { get; private init; } = DateTime.UtcNow;

    private ApiResponse() { }

    /// <summary>Cria uma resposta de sucesso com dados.</summary>
    public static ApiResponse<T> Ok(T data, string message = "Operação realizada com sucesso.") =>
        new() { Success = true, Message = message, Data = data };

    /// <summary>Cria uma resposta de falha com mensagem simples.</summary>
    public static ApiResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };

    /// <summary>Cria uma resposta de falha com lista de erros (ex: validação).</summary>
    public static ApiResponse<T> Fail(IEnumerable<string> errors, string message = "Um ou mais erros de validação ocorreram.") =>
        new() { Success = false, Message = message, Errors = [.. errors] };
}
    