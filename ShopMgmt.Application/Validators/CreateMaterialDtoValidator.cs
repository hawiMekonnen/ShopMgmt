using FluentValidation;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Validators;

public class CreateMaterialDtoValidator : AbstractValidator<CreateMaterialDto>
{
    public CreateMaterialDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
    }
}
