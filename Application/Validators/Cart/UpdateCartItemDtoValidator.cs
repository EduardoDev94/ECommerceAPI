using Application.DTOs.Cart;
using FluentValidation;

namespace Application.Validators.Cart;

public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.")
            .LessThanOrEqualTo(100).WithMessage("A quantidade máxima por item é 100.");
    }
}
