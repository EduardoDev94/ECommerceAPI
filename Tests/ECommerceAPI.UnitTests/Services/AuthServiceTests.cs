using Application.Common;
using Application.DTOs.Auth;
using Application.Services;
using Application.Services.Interfaces;
using Application.Validators.Auth;
using Core.Entities;
using Core.Repositories;
using ECommerceAPI.UnitTests.Builders;
using FluentValidation;
using NSubstitute;

namespace ECommerceAPI.UnitTests.Services;

public sealed class AuthServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenService   _tokenService   = Substitute.For<ITokenService>();
    private readonly IValidator<RegisterDto> _registerValidator = new RegisterDtoValidator();
    private readonly IValidator<LoginDto>    _loginValidator    = new LoginDtoValidator();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepository, _tokenService, _registerValidator, _loginValidator);

        _tokenService.GenerateToken(Arg.Any<User>()).Returns("mocked.jwt.token");
        _tokenService.GetExpiration().Returns(DateTime.UtcNow.AddHours(1));
    }

    // ─── RegisterAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task RegisterAsync_WithValidDto_ReturnsSuccess()
    {
        var dto = DtoFakers.ValidRegisterDto();
        _userRepository.GetByEmailAsync(dto.Email, Arg.Any<CancellationToken>()).Returns((User?)null);
        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<User>());

        var result = await _sut.RegisterAsync(dto);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("mocked.jwt.token", result.Data!.Token);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsFail()
    {
        var dto          = DtoFakers.ValidRegisterDto();
        var existingUser = new UserFaker().Generate();
        _userRepository.GetByEmailAsync(dto.Email, Arg.Any<CancellationToken>()).Returns(existingUser);

        var result = await _sut.RegisterAsync(dto);

        Assert.False(result.Success);
        Assert.Contains("e-mail", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidPassword_ThrowsValidationException()
    {
        var dto = DtoFakers.ValidRegisterDto();
        dto.Password = "weak";

        await Assert.ThrowsAsync<ValidationException>(() => _sut.RegisterAsync(dto));
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidEmail_ThrowsValidationException()
    {
        var dto = DtoFakers.ValidRegisterDto();
        dto.Email = "not-an-email";

        await Assert.ThrowsAsync<ValidationException>(() => _sut.RegisterAsync(dto));
    }

    // ─── LoginAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        const string rawPassword = "Password123!";
        var user = new UserFaker().Generate();
        user.PasswordHash = PasswordHasher.Hash(rawPassword);
        var dto = DtoFakers.ValidLoginDto(user.Email, rawPassword);

        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.LoginAsync(dto);

        Assert.True(result.Success);
        Assert.Equal("mocked.jwt.token", result.Data!.Token);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistingEmail_ReturnsFail()
    {
        var dto = DtoFakers.ValidLoginDto("naoexiste@test.com", "Password123!");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.LoginAsync(dto);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFail()
    {
        var user = new UserFaker().Generate();
        user.PasswordHash = PasswordHasher.Hash("CorrectPass1!");
        var dto = DtoFakers.ValidLoginDto(user.Email, "WrongPass9!");

        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.LoginAsync(dto);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ReturnsFail()
    {
        const string rawPassword = "Password123!";
        var user = new UserFaker().Generate();
        user.IsActive     = false;
        user.PasswordHash = PasswordHasher.Hash(rawPassword);
        var dto = DtoFakers.ValidLoginDto(user.Email, rawPassword);

        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.LoginAsync(dto);

        Assert.False(result.Success);
        Assert.Contains("desativado", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
