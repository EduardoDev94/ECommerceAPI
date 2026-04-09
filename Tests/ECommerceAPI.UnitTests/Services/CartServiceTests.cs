using Application.DTOs.Cart;
using Application.Services;
using Application.Validators.Cart;
using AutoMapper;
using Core.Entities;
using Core.Repositories;
using ECommerceAPI.UnitTests.Builders;
using ECommerceAPI.UnitTests.Fixtures;
using FluentValidation;
using NSubstitute;

namespace ECommerceAPI.UnitTests.Services;

public sealed class CartServiceTests : IClassFixture<AutoMapperFixture>
{
    private readonly ICartRepository     _cartRepository     = Substitute.For<ICartRepository>();
    private readonly ICartItemRepository _cartItemRepository = Substitute.For<ICartItemRepository>();
    private readonly IProductRepository  _productRepository  = Substitute.For<IProductRepository>();
    private readonly ICouponRepository   _couponRepository   = Substitute.For<ICouponRepository>();
    private readonly IOrderRepository    _orderRepository    = Substitute.For<IOrderRepository>();
    private readonly IUserRepository     _userRepository     = Substitute.For<IUserRepository>();
    private readonly IMapper             _mapper;
    private readonly IValidator<AddCartItemDto>    _addItemValidator    = new AddCartItemDtoValidator();
    private readonly IValidator<UpdateCartItemDto> _updateItemValidator = new UpdateCartItemDtoValidator();
    private readonly IValidator<ApplyCouponDto>    _applyCouponValidator = new ApplyCouponDtoValidator();
    private readonly CartService _sut;

    public CartServiceTests(AutoMapperFixture mapperFixture)
    {
        _mapper = mapperFixture.Mapper;
        _sut    = new CartService(
            _cartRepository, _cartItemRepository, _productRepository,
            _couponRepository, _orderRepository, _userRepository,
            _mapper, _addItemValidator, _updateItemValidator, _applyCouponValidator);
    }

    // ─── GetByUserIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByUserIdAsync_WithExistingCart_ReturnsSuccess()
    {
        var cart = new CartFaker().Generate();
        _cartRepository.GetByUserIdAsync(cart.UserId, Arg.Any<CancellationToken>()).Returns(cart);

        var result = await _sut.GetByUserIdAsync(cart.UserId);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNonExistingCart_ReturnsFail()
    {
        _cartRepository.GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Cart?)null);

        var result = await _sut.GetByUserIdAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Contains("Carrinho", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── AddItemAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task AddItemAsync_ProductNotFound_ReturnsFail()
    {
        var dto = DtoFakers.ValidAddCartItemDto();
        _productRepository.GetByIdAsync(dto.ProductId, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _sut.AddItemAsync(Guid.NewGuid(), dto);

        Assert.False(result.Success);
        Assert.Contains("Produto", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddItemAsync_WithInsufficientStock_ReturnsFail()
    {
        var product = new ProductFaker().Generate();
        product.Stock = 1;
        var dto = DtoFakers.ValidAddCartItemDto(product.Id);
        dto.Quantity = 10;

        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _sut.AddItemAsync(Guid.NewGuid(), dto);

        Assert.False(result.Success);
        Assert.Contains("Estoque", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddItemAsync_WithExistingCart_ReturnsSuccess()
    {
        var product = new ProductFaker().Generate();
        var cart    = new CartFaker().Generate();
        var dto     = DtoFakers.ValidAddCartItemDto(product.Id);
        dto.Quantity = 1;

        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _cartRepository.GetByUserIdAsync(cart.UserId, Arg.Any<CancellationToken>()).Returns(cart);
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<Cart>());

        var result = await _sut.AddItemAsync(cart.UserId, dto);

        Assert.True(result.Success);
        Assert.Contains("adicionado", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddItemAsync_WithoutExistingCart_CreatesCartAndReturnsSuccess()
    {
        var product = new ProductFaker().Generate();
        var user    = new UserFaker().Generate();
        var dto     = DtoFakers.ValidAddCartItemDto(product.Id);
        dto.Quantity = 1;

        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _cartRepository.GetByUserIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns((Cart?)null);
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _cartRepository.AddAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<Cart>());
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<Cart>());

        var result = await _sut.AddItemAsync(user.Id, dto);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task AddItemAsync_UserNotFoundAndNoCart_ReturnsFail()
    {
        var product = new ProductFaker().Generate();
        var dto     = DtoFakers.ValidAddCartItemDto(product.Id);
        dto.Quantity = 1;
        var userId  = Guid.NewGuid();

        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _cartRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns((Cart?)null);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((Core.Entities.User?)null);

        var result = await _sut.AddItemAsync(userId, dto);

        Assert.False(result.Success);
        Assert.Contains("Usuário", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── RemoveItemAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveItemAsync_WithExistingCart_ReturnsSuccess()
    {
        var product = new ProductFaker().Generate();
        var cart    = new CartFaker().GenerateWithItems(product);
        var itemId  = cart.Items.First().Id;

        _cartRepository.GetByUserIdAsync(cart.UserId, Arg.Any<CancellationToken>()).Returns(cart);
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<Cart>());

        var result = await _sut.RemoveItemAsync(cart.UserId, itemId);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task RemoveItemAsync_WithNonExistingCart_ReturnsFail()
    {
        _cartRepository.GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Cart?)null);

        var result = await _sut.RemoveItemAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result.Success);
    }

    // ─── ApplyCouponAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task ApplyCouponAsync_WithValidCoupon_ReturnsSuccess()
    {
        var cart   = new CartFaker().Generate();
        var coupon = new CouponFaker().Generate();
        var dto    = DtoFakers.ValidApplyCouponDto(coupon.Code);

        _cartRepository.GetByUserIdAsync(cart.UserId, Arg.Any<CancellationToken>()).Returns(cart);
        _couponRepository.GetByCodeAsync(coupon.Code, Arg.Any<CancellationToken>()).Returns(coupon);
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<Cart>());

        var result = await _sut.ApplyCouponAsync(cart.UserId, dto);

        Assert.True(result.Success);
        Assert.Contains("aplicado", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ApplyCouponAsync_WithCartNotFound_ReturnsFail()
    {
        _cartRepository.GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Cart?)null);

        var result = await _sut.ApplyCouponAsync(Guid.NewGuid(), DtoFakers.ValidApplyCouponDto());

        Assert.False(result.Success);
    }

    [Fact]
    public async Task ApplyCouponAsync_WithCouponNotFound_ReturnsFail()
    {
        var cart = new CartFaker().Generate();
        var dto  = DtoFakers.ValidApplyCouponDto();

        _cartRepository.GetByUserIdAsync(cart.UserId, Arg.Any<CancellationToken>()).Returns(cart);
        _couponRepository.GetByCodeAsync(dto.Code, Arg.Any<CancellationToken>()).Returns((Coupon?)null);

        var result = await _sut.ApplyCouponAsync(cart.UserId, dto);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task ApplyCouponAsync_WithInvalidCoupon_ReturnsFail()
    {
        var cart           = new CartFaker().Generate();
        var expiredCoupon  = new CouponFaker().GenerateExpired();
        var dto            = DtoFakers.ValidApplyCouponDto(expiredCoupon.Code);

        _cartRepository.GetByUserIdAsync(cart.UserId, Arg.Any<CancellationToken>()).Returns(cart);
        _couponRepository.GetByCodeAsync(expiredCoupon.Code, Arg.Any<CancellationToken>()).Returns(expiredCoupon);

        var result = await _sut.ApplyCouponAsync(cart.UserId, dto);

        Assert.False(result.Success);
        Assert.Contains("inválido", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── RemoveCouponAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveCouponAsync_WithExistingCart_ReturnsSuccess()
    {
        var coupon = new CouponFaker().Generate();
        var cart   = new CartFaker().Generate();
        cart.CouponId = coupon.Id;
        cart.Coupon   = coupon;

        _cartRepository.GetByUserIdAsync(cart.UserId, Arg.Any<CancellationToken>()).Returns(cart);
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<Cart>());

        var result = await _sut.RemoveCouponAsync(cart.UserId);

        Assert.True(result.Success);
        Assert.Contains("removido", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RemoveCouponAsync_WithNonExistingCart_ReturnsFail()
    {
        _cartRepository.GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Cart?)null);

        var result = await _sut.RemoveCouponAsync(Guid.NewGuid());

        Assert.False(result.Success);
    }

    // ─── CheckoutAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task CheckoutAsync_WithItems_CreatesOrderAndReturnsSuccess()
    {
        var product      = new ProductFaker().Generate();
        var cartWithItems = new CartFaker().GenerateWithItems(product);
        var createdOrder = new OrderFaker().Generate();

        _cartRepository.GetByUserIdAsync(cartWithItems.UserId, Arg.Any<CancellationToken>())
                       .Returns(cartWithItems);
        _cartRepository.GetWithItemsAsync(cartWithItems.Id, Arg.Any<CancellationToken>())
                       .Returns(cartWithItems);
        _orderRepository.AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>())
                        .Returns(createdOrder);
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<Cart>());

        var result = await _sut.CheckoutAsync(cartWithItems.UserId);

        Assert.True(result.Success);
        Assert.Contains("Pedido", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CheckoutAsync_WithEmptyCart_ReturnsFail()
    {
        var emptyCart = new CartFaker().Generate();

        _cartRepository.GetByUserIdAsync(emptyCart.UserId, Arg.Any<CancellationToken>())
                       .Returns(emptyCart);
        _cartRepository.GetWithItemsAsync(emptyCart.Id, Arg.Any<CancellationToken>())
                       .Returns(emptyCart);

        var result = await _sut.CheckoutAsync(emptyCart.UserId);

        Assert.False(result.Success);
        Assert.Contains("vazio", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CheckoutAsync_CartNotFound_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        _cartRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns((Cart?)null);
        _cartRepository.GetWithItemsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Cart?)null);

        var result = await _sut.CheckoutAsync(userId);

        Assert.False(result.Success);
    }
}
