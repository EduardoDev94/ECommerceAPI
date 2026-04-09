using Application.Common;
using Application.DTOs.User;

namespace Application.Services.Interfaces;

public interface IUserService
{
    Task<PagedResponse<UserResponseDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponseDto>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserResponseDto>> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
