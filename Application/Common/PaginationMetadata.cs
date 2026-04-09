namespace Application.Common;

/// <summary>
/// Metadados de paginação incluídos em PagedResponse.
/// Todos os valores derivados (TotalPages, HasNextPage, HasPreviousPage)
/// são calculados automaticamente no construtor.
/// </summary>
public sealed class PaginationMetadata
{
    /// <summary>Número da página atual (base 1).</summary>
    public int Page { get; }

    /// <summary>Quantidade de itens por página.</summary>
    public int PageSize { get; }

    /// <summary>Total de itens em todas as páginas.</summary>
    public int TotalCount { get; }

    /// <summary>Total de páginas calculado a partir de TotalCount e PageSize.</summary>
    public int TotalPages { get; }

    /// <summary>Indica se existe uma próxima página.</summary>
    public bool HasNextPage { get; }

    /// <summary>Indica se existe uma página anterior.</summary>
    public bool HasPreviousPage { get; }

    public PaginationMetadata() { }

    public PaginationMetadata(int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNextPage = page < TotalPages;
        HasPreviousPage = page > 1;
    }
}
