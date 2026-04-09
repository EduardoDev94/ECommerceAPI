using Application.DTOs.Product;
using Application.Validators.Product;
using ECommerceAPI.UnitTests.Builders;

namespace ECommerceAPI.UnitTests.Validators;

public sealed class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidDto_ShouldHaveNoErrors()
    {
        var dto    = DtoFakers.ValidCreateProductDto();
        var result = await _validator.ValidateAsync(dto);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("AB")] // < 3 chars
    public async Task Validate_WithInvalidName_ShouldHaveNameError(string name)
    {
        var dto  = DtoFakers.ValidCreateProductDto();
        dto.Name = name;

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Name));
    }

    [Fact]
    public async Task Validate_WithEmptyDescription_ShouldHaveDescriptionError()
    {
        var dto         = DtoFakers.ValidCreateProductDto();
        dto.Description = string.Empty;

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Description));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Validate_WithNonPositivePrice_ShouldHavePriceError(decimal price)
    {
        var dto   = DtoFakers.ValidCreateProductDto();
        dto.Price = price;

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Price));
    }

    [Fact]
    public async Task Validate_WithNegativeStock_ShouldHaveStockError()
    {
        var dto   = DtoFakers.ValidCreateProductDto();
        dto.Stock = -1;

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Stock));
    }

    [Fact]
    public async Task Validate_WithZeroStock_ShouldHaveNoErrors()
    {
        var dto   = DtoFakers.ValidCreateProductDto();
        dto.Stock = 0;

        var result = await _validator.ValidateAsync(dto);

        Assert.True(result.IsValid);
    }
}
