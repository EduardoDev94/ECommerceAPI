using Application.Common;
using Application.DTOs.Auth;
using Application.Services.Interfaces;
using Core.Entities;
using Core.Repositories;
using FluentValidation;

namespace Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        await _registerValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var existing = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existing is not null)
            return ApiResponse<AuthResponseDto>.Fail("Já existe um usuário com este e-mail.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = dto.Role
        };

        var created = await _userRepository.AddAsync(user, cancellationToken);
        return ApiResponse<AuthResponseDto>.Ok(BuildResponse(created), "Usuário registrado com sucesso.");
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        await _loginValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (user is null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            return ApiResponse<AuthResponseDto>.Fail("E-mail ou senha inválidos.");

        if (!user.IsActive)
            return ApiResponse<AuthResponseDto>.Fail("Usuário desativado. Entre em contato com o suporte.");

        return ApiResponse<AuthResponseDto>.Ok(BuildResponse(user));
    }

    private AuthResponseDto BuildResponse(User user) => new()
    {
        Token = _tokenService.GenerateToken(user),
        Name = user.Name,
        Email = user.Email,
        Role = user.Role.ToString(),
        ExpiresAt = _tokenService.GetExpiration()
    };
}
