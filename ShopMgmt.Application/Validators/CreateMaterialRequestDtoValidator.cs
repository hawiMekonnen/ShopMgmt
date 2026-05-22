using FluentValidation;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Validators;

public class CreateMaterialRequestDtoValidator : AbstractValidator<CreateMaterialRequestDto>
{
    public CreateMaterialRequestDtoValidator()
    {
        RuleFor(x => x.MaterialId).GreaterThan(0);
        RuleFor(x => x.ShopId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.AircraftOrWorkOrder)
            .NotEmpty()
            .WithMessage("Work order is required.")
            .MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes != null);
    }
}
