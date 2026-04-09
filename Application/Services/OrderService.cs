using Application.Common;
using Application.DTOs.Order;
using Application.Services.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Repositories;
using FluentValidation;

namespace Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateOrderStatusDto> _updateStatusValidator;

    public OrderService(
        IOrderRepository orderRepository,
        IMapper mapper,
        IValidator<UpdateOrderStatusDto> updateStatusValidator)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _updateStatusValidator = updateStatusValidator;
    }

    public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllOrdersAsync(cancellationToken);
        return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(_mapper.Map<IEnumerable<OrderResponseDto>>(orders));
    }

    public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken);
        return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(_mapper.Map<IEnumerable<OrderResponseDto>>(orders));
    }

    public async Task<ApiResponse<OrderResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithItemsAsync(id, cancellationToken);
        if (order is null)
            return ApiResponse<OrderResponseDto>.Fail("Pedido não encontrado.");

        return ApiResponse<OrderResponseDto>.Ok(_mapper.Map<OrderResponseDto>(order));
    }

    public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var orderStatus))
            return ApiResponse<IEnumerable<OrderResponseDto>>.Fail($"Status inválido: '{status}'. Valores aceitos: {string.Join(", ", Enum.GetNames<OrderStatus>())}.");

        var orders = await _orderRepository.GetByStatusAsync(orderStatus, cancellationToken);
        return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(_mapper.Map<IEnumerable<OrderResponseDto>>(orders));
    }

    public async Task<ApiResponse<OrderResponseDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await _updateStatusValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return ApiResponse<OrderResponseDto>.Fail(validation.Errors[0].ErrorMessage);

        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return ApiResponse<OrderResponseDto>.Fail("Pedido não encontrado.");

        if (!Enum.TryParse<OrderStatus>(dto.Status, ignoreCase: true, out var newStatus))
            return ApiResponse<OrderResponseDto>.Fail($"Status inválido: '{dto.Status}'. Valores aceitos: {string.Join(", ", Enum.GetNames<OrderStatus>())}.");

        order.UpdateStatus(newStatus);

        var updated = await _orderRepository.UpdateAsync(order, cancellationToken);
        return ApiResponse<OrderResponseDto>.Ok(_mapper.Map<OrderResponseDto>(updated), "Status do pedido atualizado com sucesso.");
    }
}
