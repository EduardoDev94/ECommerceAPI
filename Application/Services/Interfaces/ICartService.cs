using Application.Common;
using Application.DTOs.Cart;
using Application.DTOs.Order;

namespace Application.Services.Interfaces;

public interface ICartService
{
    Task<ApiResponse<CartResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<CartResponseDto>> AddItemAsync(Guid userId, AddCartItemDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<CartResponseDto>> UpdateItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<CartResponseDto>> RemoveItemAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken = default);
    Task<ApiResponse<CartResponseDto>> ApplyCouponAsync(Guid userId, ApplyCouponDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<CartResponseDto>> RemoveCouponAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<OrderResponseDto>> CheckoutAsync(Guid userId, CancellationToken cancellationToken = default);
}
