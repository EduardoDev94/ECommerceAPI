using Application.DTOs.Coupon;
using FluentValidation;

namespace Application.Validators.Coupon;

public class UpdateCouponDtoValidator : AbstractValidator<UpdateCouponDto>
{
    public UpdateCouponDtoValidator()
    {
        RuleFor(x => x.ExpirationDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("A data de expiração deve ser uma data futura.");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0).WithMessage("O limite de uso deve ser maior que zero.");
    }
}
