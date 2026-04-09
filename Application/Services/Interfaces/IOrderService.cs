using Application.Common;
using Application.DTOs.Order;

namespace Application.Services.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<OrderResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<ApiResponse<OrderResponseDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default);
}
