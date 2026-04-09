namespace Application.Common;

/// <summary>
/// Envelope padrão para respostas paginadas da API.
/// Segue o mesmo contrato de ApiResponse, adicionando os metadados de paginação.
/// </summary>
public sealed class PagedResponse<T>
{
    /// <summary>Indica se a operação foi concluída com sucesso.</summary>
    public bool Success { get; private init; }

    /// <summary>Mensagem descritiva do resultado.</summary>
    public string Message { get; private init; } = string.Empty;

    /// <summary>Coleção de itens da página atual.</summary>
    public IEnumerable<T> Data { get; private init; } = [];

    /// <summary>Metadados de paginação (página, total, próxima, anterior).</summary>
    public PaginationMetadata Pagination { get; private init; } = new();

    /// <summary>Lista de erros de validação ou de negócio.</summary>
    public IReadOnlyList<string> Errors { get; private init; } = [];

    /// <summary>Momento UTC em que a resposta foi gerada.</summary>
    public DateTime Timestamp { get; private init; } = DateTime.UtcNow;

    private PagedResponse() { }

    /// <summary>Cria uma resposta paginada de sucesso.</summary>
    public static PagedResponse<T> Ok(
        IEnumerable<T> data,
        int page,
        int pageSize,
        int totalCount,
        string message = "Operação realizada com sucesso.") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = new PaginationMetadata(page, pageSize, totalCount)
        };

    /// <summary>Cria uma resposta paginada de falha.</summary>
    public static PagedResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
