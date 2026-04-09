using Application.Common;
using Application.DTOs.Coupon;
using Application.Services.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Repositories;
using FluentValidation;

namespace Application.Services;

public sealed class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCouponDto> _createValidator;
    private readonly IValidator<UpdateCouponDto> _updateValidator;

    public CouponService(
        ICouponRepository couponRepository,
        IMapper mapper,
        IValidator<CreateCouponDto> createValidator,
        IValidator<UpdateCouponDto> updateValidator)
    {
        _couponRepository = couponRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var coupons = await _couponRepository.GetAllAsync(cancellationToken);
        return ApiResponse<IEnumerable<CouponResponseDto>>.Ok(_mapper.Map<IEnumerable<CouponResponseDto>>(coupons));
    }

    public async Task<ApiResponse<CouponResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var coupon = await _couponRepository.GetByIdAsync(id, cancellationToken);
        if (coupon is null)
            return ApiResponse<CouponResponseDto>.Fail("Cupom não encontrado.");

        return ApiResponse<CouponResponseDto>.Ok(_mapper.Map<CouponResponseDto>(coupon));
    }

    public async Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var coupons = await _couponRepository.GetActiveCouponsAsync(cancellationToken);
        return ApiResponse<IEnumerable<CouponResponseDto>>.Ok(_mapper.Map<IEnumerable<CouponResponseDto>>(coupons));
    }

    public async Task<ApiResponse<CouponResponseDto>> CreateAsync(CreateCouponDto dto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var existing = await _couponRepository.GetByCodeAsync(dto.Code, cancellationToken);
        if (existing is not null)
            return ApiResponse<CouponResponseDto>.Fail("Já existe um cupom com este código.");

        var coupon = _mapper.Map<Coupon>(dto);
        var created = await _couponRepository.AddAsync(coupon, cancellationToken);
        return ApiResponse<CouponResponseDto>.Ok(_mapper.Map<CouponResponseDto>(created), "Cupom criado com sucesso.");
    }

    public async Task<ApiResponse<CouponResponseDto>> UpdateAsync(Guid id, UpdateCouponDto dto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var coupon = await _couponRepository.GetByIdAsync(id, cancellationToken);
        if (coupon is null)
            return ApiResponse<CouponResponseDto>.Fail("Cupom não encontrado.");

        _mapper.Map(dto, coupon);
        coupon.UpdatedAt = DateTime.UtcNow;

        var updated = await _couponRepository.UpdateAsync(coupon, cancellationToken);
        return ApiResponse<CouponResponseDto>.Ok(_mapper.Map<CouponResponseDto>(updated), "Cupom atualizado com sucesso.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _couponRepository.ExistsAsync(id, cancellationToken);
        if (!exists)
            return ApiResponse<bool>.Fail("Cupom não encontrado.");

        await _couponRepository.DeleteAsync(id, cancellationToken);
        return ApiResponse<bool>.Ok(true, "Cupom removido com sucesso.");
    }
}
