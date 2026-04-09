using Application.Common;
using Application.DTOs.User;
using Application.Services.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Repositories;
using FluentValidation;

namespace Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateUserDto> _createValidator;
    private readonly IValidator<UpdateUserDto> _updateValidator;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<CreateUserDto> createValidator,
        IValidator<UpdateUserDto> updateValidator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResponse<UserResponseDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var total = users.Count();
        var paged = users.Skip((page - 1) * pageSize).Take(pageSize);
        var dtos = _mapper.Map<IEnumerable<UserResponseDto>>(paged);
        return PagedResponse<UserResponseDto>.Ok(dtos, page, pageSize, total);
    }

    public async Task<ApiResponse<UserResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return ApiResponse<UserResponseDto>.Fail("Usuário não encontrado.");

        return ApiResponse<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(user));
    }

    public async Task<ApiResponse<UserResponseDto>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var existing = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existing is not null)
            return ApiResponse<UserResponseDto>.Fail("Já existe um usuário com este e-mail.");

        var user = _mapper.Map<User>(dto);
        var created = await _userRepository.AddAsync(user, cancellationToken);
        return ApiResponse<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(created), "Usuário criado com sucesso.");
    }

    public async Task<ApiResponse<UserResponseDto>> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return ApiResponse<UserResponseDto>.Fail("Usuário não encontrado.");

        _mapper.Map(dto, user);
        user.UpdatedAt = DateTime.UtcNow;

        var updated = await _userRepository.UpdateAsync(user, cancellationToken);
        return ApiResponse<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(updated), "Usuário atualizado com sucesso.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _userRepository.ExistsAsync(id, cancellationToken);
        if (!exists)
            return ApiResponse<bool>.Fail("Usuário não encontrado.");

        await _userRepository.DeleteAsync(id, cancellationToken);
        return ApiResponse<bool>.Ok(true, "Usuário removido com sucesso.");
    }
}
