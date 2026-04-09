using Application.Common;
using Application.DTOs.Coupon;

namespace Application.Services.Interfaces;

public interface ICouponService
{
    Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<CouponResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<CouponResponseDto>> CreateAsync(CreateCouponDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<CouponResponseDto>> UpdateAsync(Guid id, UpdateCouponDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
