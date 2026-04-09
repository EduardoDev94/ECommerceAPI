using Application.DTOs.Order;
using FluentValidation;

namespace Application.Validators.Order;

public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    private static readonly string[] ValidStatuses =
        ["Pending", "Paid", "Processing", "Shipped", "Delivered", "Cancelled"];

    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("O status do pedido é obrigatório.")
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage($"Status inválido. Valores aceitos: {string.Join(", ", ValidStatuses)}.");
    }
}
