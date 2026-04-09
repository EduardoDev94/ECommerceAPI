using Application.DTOs.Cart;
using FluentValidation;

namespace Application.Validators.Cart;

public class ApplyCouponDtoValidator : AbstractValidator<ApplyCouponDto>
{
    public ApplyCouponDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("O código do cupom é obrigatório.")
            .MaximumLength(50).WithMessage("O código deve ter no máximo 50 caracteres.")
            .Matches(@"^[A-Z0-9_\-]+$").WithMessage("O código deve conter apenas letras maiúsculas, números, hífen e underscore.");
    }
}
