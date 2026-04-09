using Application.DTOs.Coupon;
using FluentValidation;

namespace Application.Validators.Coupon;

public class CreateCouponDtoValidator : AbstractValidator<CreateCouponDto>
{
    public CreateCouponDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("O código do cupom é obrigatório.")
            .MinimumLength(3).WithMessage("O código deve ter pelo menos 3 caracteres.")
            .MaximumLength(50).WithMessage("O código deve ter no máximo 50 caracteres.")
            .Matches(@"^[A-Z0-9_\-]+$").WithMessage("O código deve conter apenas letras maiúsculas, números, hífen e underscore.");

        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0).WithMessage("O percentual de desconto deve ser maior que zero.")
            .LessThanOrEqualTo(100).WithMessage("O percentual de desconto não pode ser maior que 100.");

        RuleFor(x => x.ExpirationDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("A data de expiração deve ser uma data futura.");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0).WithMessage("O limite de uso deve ser maior que zero.");
    }
}
