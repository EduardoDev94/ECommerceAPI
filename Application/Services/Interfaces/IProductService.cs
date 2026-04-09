using Application.Common;
using Application.DTOs.Product;

namespace Application.Services.Interfaces;

public interface IProductService
{
    Task<PagedResponse<ProductResponseDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductResponseDto>>> GetInStockAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductResponseDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductResponseDto>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
