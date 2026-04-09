using Application.DTOs.Product;
using Application.Services;
using Application.Validators.Product;
using AutoMapper;
using Core.Entities;
using Core.Repositories;
using ECommerceAPI.UnitTests.Builders;
using ECommerceAPI.UnitTests.Fixtures;
using FluentValidation;
using NSubstitute;

namespace ECommerceAPI.UnitTests.Services;

public sealed class ProductServiceTests : IClassFixture<AutoMapperFixture>
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IMapper            _mapper;
    private readonly IValidator<CreateProductDto> _createValidator = new CreateProductDtoValidator();
    private readonly IValidator<UpdateProductDto> _updateValidator = new UpdateProductDtoValidator();
    private readonly ProductService _sut;

    public ProductServiceTests(AutoMapperFixture mapperFixture)
    {
        _mapper = mapperFixture.Mapper;
        _sut    = new ProductService(_productRepository, _mapper, _createValidator, _updateValidator);
    }

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WithProducts_ReturnsPagedResponse()
    {
        var products = new ProductFaker().Generate(6);
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _sut.GetAllAsync(1, 10);

        Assert.True(result.Success);
        Assert.Equal(6, result.Pagination.TotalCount);
    }

    [Fact]
    public async Task GetAllAsync_SecondPage_ReturnsCorrectSlice()
    {
        var products = new ProductFaker().Generate(10);
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _sut.GetAllAsync(2, 4);

        Assert.True(result.Success);
        Assert.Equal(4, result.Data!.Count());
        Assert.Equal(2, result.Pagination.Page);
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithExistingProduct_ReturnsSuccess()
    {
        var product = new ProductFaker().Generate();
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _sut.GetByIdAsync(product.Id);

        Assert.True(result.Success);
        Assert.Equal(product.Name, result.Data!.Name);
        Assert.Equal(product.Price, result.Data.Price);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingProduct_ReturnsFail()
    {
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Contains("não encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── GetInStockAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetInStockAsync_ReturnsOnlyInStockProducts()
    {
        var inStock = new ProductFaker().Generate(3);
        _productRepository.GetInStockAsync(Arg.Any<CancellationToken>()).Returns(inStock);

        var result = await _sut.GetInStockAsync();

        Assert.True(result.Success);
        Assert.Equal(3, result.Data!.Count());
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidDto_ReturnsSuccess()
    {
        var dto = DtoFakers.ValidCreateProductDto();
        _productRepository.GetByNameAsync(dto.Name, Arg.Any<CancellationToken>()).Returns((Product?)null);
        _productRepository.AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
                          .Returns(ci => ci.Arg<Product>());

        var result = await _sut.CreateAsync(dto);

        Assert.True(result.Success);
        Assert.Equal(dto.Name, result.Data!.Name);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ReturnsFail()
    {
        var dto      = DtoFakers.ValidCreateProductDto();
        var existing = new ProductFaker().Generate();
        _productRepository.GetByNameAsync(dto.Name, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _sut.CreateAsync(dto);

        Assert.False(result.Success);
        Assert.Contains("nome", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithNegativePrice_ThrowsValidationException()
    {
        var dto = DtoFakers.ValidCreateProductDto();
        dto.Price = -1;

        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _sut.CreateAsync(dto));
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithExistingProduct_ReturnsSuccess()
    {
        var product = new ProductFaker().Generate();
        var dto     = DtoFakers.ValidUpdateProductDto();
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
                          .Returns(ci => ci.Arg<Product>());

        var result = await _sut.UpdateAsync(product.Id, dto);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingProduct_ReturnsFail()
    {
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), DtoFakers.ValidUpdateProductDto());

        Assert.False(result.Success);
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithExistingProduct_ReturnsSuccess()
    {
        var id = Guid.NewGuid();
        _productRepository.ExistsAsync(id, Arg.Any<CancellationToken>()).Returns(true);
        _productRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

        var result = await _sut.DeleteAsync(id);

        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingProduct_ReturnsFail()
    {
        _productRepository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.False(result.Success);
    }
}
