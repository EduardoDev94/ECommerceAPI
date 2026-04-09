using Application.Common;
using Application.DTOs.Cart;
using Application.DTOs.Order;
using Application.Services.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Repositories;
using FluentValidation;

namespace Application.Services;

public sealed class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<AddCartItemDto> _addItemValidator;
    private readonly IValidator<UpdateCartItemDto> _updateItemValidator;
    private readonly IValidator<ApplyCouponDto> _applyCouponValidator;

    public CartService(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IProductRepository productRepository,
        ICouponRepository couponRepository,
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<AddCartItemDto> addItemValidator,
        IValidator<UpdateCartItemDto> updateItemValidator,
        IValidator<ApplyCouponDto> applyCouponValidator)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
        _couponRepository = couponRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _addItemValidator = addItemValidator;
        _updateItemValidator = updateItemValidator;
        _applyCouponValidator = applyCouponValidator;
    }

    public async Task<ApiResponse<CartResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null)
            return ApiResponse<CartResponseDto>.Fail("Carrinho não encontrado.");

        return ApiResponse<CartResponseDto>.Ok(_mapper.Map<CartResponseDto>(cart));
    }

    public async Task<ApiResponse<CartResponseDto>> AddItemAsync(Guid userId, AddCartItemDto dto, CancellationToken cancellationToken = default)
    {
        await _addItemValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken);
        if (product is null)
            return ApiResponse<CartResponseDto>.Fail("Produto não encontrado.");

        if (product.Stock < dto.Quantity)
            return ApiResponse<CartResponseDto>.Fail($"Estoque insuficiente. Disponível: {product.Stock}.");

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                return ApiResponse<CartResponseDto>.Fail("Usuário não encontrado.");

            cart = new Cart { UserId = userId };
            cart = await _cartRepository.AddAsync(cart, cancellationToken);
        }

        var newItem = _mapper.Map<CartItem>(dto);
        newItem.CartId = cart.Id;
        newItem.Product = product;

        cart.AddItem(newItem);
        cart.UpdatedAt = DateTime.UtcNow;

        var updated = await _cartRepository.UpdateAsync(cart, cancellationToken);
        return ApiResponse<CartResponseDto>.Ok(_mapper.Map<CartResponseDto>(updated), "Item adicionado ao carrinho.");
    }

    public async Task<ApiResponse<CartResponseDto>> UpdateItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto, CancellationToken cancellationToken = default)
    {
        await _updateItemValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var cart = await _cartRepository.GetWithItemsAsync(
            (await _cartRepository.GetByUserIdAsync(userId, cancellationToken))?.Id ?? Guid.Empty,
            cancellationToken);

        if (cart is null)
            return ApiResponse<CartResponseDto>.Fail("Carrinho não encontrado.");

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item is null)
            return ApiResponse<CartResponseDto>.Fail("Item não encontrado no carrinho.");

        if (item.Product!.Stock < dto.Quantity)
            return ApiResponse<CartResponseDto>.Fail($"Estoque insuficiente. Disponível: {item.Product.Stock}.");

        item.Quantity = dto.Quantity;
        item.UpdatedAt = DateTime.UtcNow;

        cart.CalculateTotals();
        cart.UpdatedAt = DateTime.UtcNow;

        var updated = await _cartRepository.UpdateAsync(cart, cancellationToken);
        return ApiResponse<CartResponseDto>.Ok(_mapper.Map<CartResponseDto>(updated), "Item atualizado.");
    }

    public async Task<ApiResponse<CartResponseDto>> RemoveItemAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null)
            return ApiResponse<CartResponseDto>.Fail("Carrinho não encontrado.");

        cart.RemoveItem(cartItemId);
        cart.UpdatedAt = DateTime.UtcNow;

        var updated = await _cartRepository.UpdateAsync(cart, cancellationToken);
        return ApiResponse<CartResponseDto>.Ok(_mapper.Map<CartResponseDto>(updated), "Item removido do carrinho.");
    }

    public async Task<ApiResponse<CartResponseDto>> ApplyCouponAsync(Guid userId, ApplyCouponDto dto, CancellationToken cancellationToken = default)
    {
        await _applyCouponValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null)
            return ApiResponse<CartResponseDto>.Fail("Carrinho não encontrado.");

        var coupon = await _couponRepository.GetByCodeAsync(dto.Code, cancellationToken);
        if (coupon is null)
            return ApiResponse<CartResponseDto>.Fail("Cupom não encontrado.");

        if (!coupon.IsValid())
            return ApiResponse<CartResponseDto>.Fail("Cupom inválido, expirado ou fora do limite de uso.");

        cart.CouponId = coupon.Id;
        cart.Coupon = coupon;
        cart.CalculateTotals();
        cart.UpdatedAt = DateTime.UtcNow;

        var updated = await _cartRepository.UpdateAsync(cart, cancellationToken);
        return ApiResponse<CartResponseDto>.Ok(_mapper.Map<CartResponseDto>(updated), "Cupom aplicado com sucesso.");
    }

    public async Task<ApiResponse<CartResponseDto>> RemoveCouponAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null)
            return ApiResponse<CartResponseDto>.Fail("Carrinho não encontrado.");

        cart.CouponId = null;
        cart.Coupon = null;
        cart.CalculateTotals();
        cart.UpdatedAt = DateTime.UtcNow;

        var updated = await _cartRepository.UpdateAsync(cart, cancellationToken);
        return ApiResponse<CartResponseDto>.Ok(_mapper.Map<CartResponseDto>(updated), "Cupom removido do carrinho.");
    }

    public async Task<ApiResponse<OrderResponseDto>> CheckoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetWithItemsAsync(
            (await _cartRepository.GetByUserIdAsync(userId, cancellationToken))?.Id ?? Guid.Empty,
            cancellationToken);

        if (cart is null)
            return ApiResponse<OrderResponseDto>.Fail("Carrinho não encontrado.");

        if (!cart.Items.Any())
            return ApiResponse<OrderResponseDto>.Fail("O carrinho está vazio.");

        var order = new Order
        {
            UserId = userId,
            TotalAmount = cart.TotalAmount,
            DiscountAmount = cart.DiscountAmount,
            FinalAmount = cart.FinalAmount,
            CouponCode = cart.Coupon?.Code,
            Status = OrderStatus.Pending
        };

        foreach (var item in cart.Items)
        {
            var orderItem = OrderItem.CreateFromCartItem(item);
            orderItem.OrderId = order.Id;
            order.Items.Add(orderItem);
        }

        var createdOrder = await _orderRepository.AddAsync(order, cancellationToken);

        if (cart.Coupon is not null)
        {
            cart.Coupon.IncrementUsage();
            await _couponRepository.UpdateAsync(cart.Coupon, cancellationToken);
        }

        cart.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateAsync(cart, cancellationToken);

        return ApiResponse<OrderResponseDto>.Ok(_mapper.Map<OrderResponseDto>(createdOrder), "Pedido criado com sucesso.");
    }
}
