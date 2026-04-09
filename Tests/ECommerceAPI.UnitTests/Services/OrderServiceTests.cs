using Application.DTOs.Order;
using Application.Services;
using Application.Validators.Order;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Repositories;
using ECommerceAPI.UnitTests.Builders;
using ECommerceAPI.UnitTests.Fixtures;
using FluentValidation;
using NSubstitute;

namespace ECommerceAPI.UnitTests.Services;

public sealed class OrderServiceTests : IClassFixture<AutoMapperFixture>
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IMapper          _mapper;
    private readonly IValidator<UpdateOrderStatusDto> _updateStatusValidator = new UpdateOrderStatusDtoValidator();
    private readonly OrderService _sut;

    public OrderServiceTests(AutoMapperFixture mapperFixture)
    {
        _mapper = mapperFixture.Mapper;
        _sut    = new OrderService(_orderRepository, _mapper, _updateStatusValidator);
    }

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var orders = new OrderFaker().Generate(5);
        _orderRepository.GetAllOrdersAsync(Arg.Any<CancellationToken>()).Returns(orders);

        var result = await _sut.GetAllAsync();

        Assert.True(result.Success);
        Assert.Equal(5, result.Data!.Count());
    }

    // ─── GetByUserIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserOrders()
    {
        var userId = Guid.NewGuid();
        var orders = new OrderFaker().Generate(3);
        _orderRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(orders);

        var result = await _sut.GetByUserIdAsync(userId);

        Assert.True(result.Success);
        Assert.Equal(3, result.Data!.Count());
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithExistingOrder_ReturnsSuccess()
    {
        var order = new OrderFaker().Generate();
        _orderRepository.GetWithItemsAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _sut.GetByIdAsync(order.Id);

        Assert.True(result.Success);
        Assert.Equal(order.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingOrder_ReturnsFail()
    {
        _orderRepository.GetWithItemsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Order?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Contains("não encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── GetByStatusAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByStatusAsync_WithValidStatus_ReturnsFilteredOrders()
    {
        var orders = new OrderFaker().Generate(2);
        _orderRepository.GetByStatusAsync(OrderStatus.Paid, Arg.Any<CancellationToken>()).Returns(orders);

        var result = await _sut.GetByStatusAsync("Paid");

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task GetByStatusAsync_WithInvalidStatus_ReturnsFail()
    {
        var result = await _sut.GetByStatusAsync("StatusInexistente");

        Assert.False(result.Success);
        Assert.Contains("inválido", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetByStatusAsync_IsCaseInsensitive()
    {
        var orders = new OrderFaker().Generate(1);
        _orderRepository.GetByStatusAsync(OrderStatus.Pending, Arg.Any<CancellationToken>()).Returns(orders);

        var result = await _sut.GetByStatusAsync("pending");

        Assert.True(result.Success);
    }

    // ─── UpdateStatusAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatusAsync_WithValidData_ReturnsSuccess()
    {
        var order = new OrderFaker().Generate();
        var dto   = DtoFakers.ValidUpdateOrderStatusDto("Shipped");

        _orderRepository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _orderRepository.UpdateAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>())
                        .Returns(ci => ci.Arg<Order>());

        var result = await _sut.UpdateStatusAsync(order.Id, dto);

        Assert.True(result.Success);
        Assert.Equal("Shipped", result.Data!.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_WithNonExistingOrder_ReturnsFail()
    {
        _orderRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Order?)null);

        var result = await _sut.UpdateStatusAsync(Guid.NewGuid(), DtoFakers.ValidUpdateOrderStatusDto("Paid"));

        Assert.False(result.Success);
    }

    [Fact]
    public async Task UpdateStatusAsync_WithInvalidStatus_ReturnsFail()
    {
        var order = new OrderFaker().Generate();
        _orderRepository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var dto = DtoFakers.ValidUpdateOrderStatusDto("StatusInvalido");

        var result = await _sut.UpdateStatusAsync(order.Id, dto);

        Assert.False(result.Success);
    }
}
