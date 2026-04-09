using Application.DTOs.Auth;
using Application.Validators.Auth;
using Core.Enums;
using ECommerceAPI.UnitTests.Builders;

namespace ECommerceAPI.UnitTests.Validators;

public sealed class RegisterDtoValidatorTests
{
    private readonly RegisterDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidDto_ShouldHaveNoErrors()
    {
        var dto    = DtoFakers.ValidRegisterDto();
        var result = await _validator.ValidateAsync(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public async Task Validate_WithInvalidName_ShouldHaveNameError(string name)
    {
        var dto  = DtoFakers.ValidRegisterDto();
        dto.Name = name;

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    public async Task Validate_WithInvalidEmail_ShouldHaveEmailError(string email)
    {
        var dto   = DtoFakers.ValidRegisterDto();
        dto.Email = email;

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short1")]        // < 8 chars
    [InlineData("nouppercase1")] // sem maiúscula
    [InlineData("NoDigitsHere")] // sem número
    public async Task Validate_WithInvalidPassword_ShouldHavePasswordError(string password)
    {
        var dto      = DtoFakers.ValidRegisterDto();
        dto.Password = password;

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Password));
    }

    [Fact]
    public async Task Validate_WithAdminRole_ShouldHaveNoErrors()
    {
        var dto  = DtoFakers.ValidRegisterDto();
        dto.Role = UserRole.Admin;

        var result = await _validator.ValidateAsync(dto);

        Assert.True(result.IsValid);
    }
}
