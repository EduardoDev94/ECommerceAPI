using Application.DTOs.Coupon;
using Application.Services;
using Application.Validators.Coupon;
using AutoMapper;
using Core.Entities;
using Core.Repositories;
using ECommerceAPI.UnitTests.Builders;
using ECommerceAPI.UnitTests.Fixtures;
using FluentValidation;
using NSubstitute;

namespace ECommerceAPI.UnitTests.Services;

public sealed class CouponServiceTests : IClassFixture<AutoMapperFixture>
{
    private readonly ICouponRepository _couponRepository = Substitute.For<ICouponRepository>();
    private readonly IMapper           _mapper;
    private readonly IValidator<CreateCouponDto> _createValidator = new CreateCouponDtoValidator();
    private readonly IValidator<UpdateCouponDto> _updateValidator = new UpdateCouponDtoValidator();
    private readonly CouponService _sut;

    public CouponServiceTests(AutoMapperFixture mapperFixture)
    {
        _mapper = mapperFixture.Mapper;
        _sut    = new CouponService(_couponRepository, _mapper, _createValidator, _updateValidator);
    }

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsMappedCoupons()
    {
        var coupons = new CouponFaker().Generate(4);
        _couponRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(coupons);

        var result = await _sut.GetAllAsync();

        Assert.True(result.Success);
        Assert.Equal(4, result.Data!.Count());
    }

    // ─── GetActiveAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetActiveAsync_ReturnsActiveCoupons()
    {
        var active = new CouponFaker().Generate(2);
        _couponRepository.GetActiveCouponsAsync(Arg.Any<CancellationToken>()).Returns(active);

        var result = await _sut.GetActiveAsync();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count());
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithExistingCoupon_ReturnsSuccess()
    {
        var coupon = new CouponFaker().Generate();
        _couponRepository.GetByIdAsync(coupon.Id, Arg.Any<CancellationToken>()).Returns(coupon);

        var result = await _sut.GetByIdAsync(coupon.Id);

        Assert.True(result.Success);
        Assert.Equal(coupon.Code, result.Data!.Code);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingCoupon_ReturnsFail()
    {
        _couponRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Coupon?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Contains("não encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidDto_ReturnsSuccess()
    {
        var dto = DtoFakers.ValidCreateCouponDto();
        _couponRepository.GetByCodeAsync(dto.Code, Arg.Any<CancellationToken>()).Returns((Coupon?)null);
        _couponRepository.AddAsync(Arg.Any<Coupon>(), Arg.Any<CancellationToken>())
                         .Returns(ci => ci.Arg<Coupon>());

        var result = await _sut.CreateAsync(dto);

        Assert.True(result.Success);
        Assert.Equal(dto.Code, result.Data!.Code);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ReturnsFail()
    {
        var dto      = DtoFakers.ValidCreateCouponDto();
        var existing = new CouponFaker().Generate();
        _couponRepository.GetByCodeAsync(dto.Code, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _sut.CreateAsync(dto);

        Assert.False(result.Success);
        Assert.Contains("código", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithExistingCoupon_ReturnsSuccess()
    {
        var coupon = new CouponFaker().Generate();
        var dto    = DtoFakers.ValidUpdateCouponDto();
        _couponRepository.GetByIdAsync(coupon.Id, Arg.Any<CancellationToken>()).Returns(coupon);
        _couponRepository.UpdateAsync(Arg.Any<Coupon>(), Arg.Any<CancellationToken>())
                         .Returns(ci => ci.Arg<Coupon>());

        var result = await _sut.UpdateAsync(coupon.Id, dto);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingCoupon_ReturnsFail()
    {
        _couponRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Coupon?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), DtoFakers.ValidUpdateCouponDto());

        Assert.False(result.Success);
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithExistingCoupon_ReturnsSuccess()
    {
        var id = Guid.NewGuid();
        _couponRepository.ExistsAsync(id, Arg.Any<CancellationToken>()).Returns(true);
        _couponRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

        var result = await _sut.DeleteAsync(id);

        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingCoupon_ReturnsFail()
    {
        _couponRepository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.False(result.Success);
    }
}
