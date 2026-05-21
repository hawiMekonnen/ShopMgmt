using FluentValidation;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Validators;

public class UpdateMaterialDtoValidator : AbstractValidator<UpdateMaterialDto>
{
    public UpdateMaterialDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
    }
}
