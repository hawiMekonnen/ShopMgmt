using FluentValidation;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Validators;

public class CreateServiceabilityCheckDtoValidator : AbstractValidator<CreateServiceabilityCheckDto>
{
    public CreateServiceabilityCheckDtoValidator()
    {
        RuleFor(x => x.BatchId).GreaterThan(0).WithMessage("BatchId must be a positive integer.");
        RuleFor(x => x.TechnicianId).GreaterThan(0).WithMessage("TechnicianId must be a positive integer.");
        RuleFor(x => x.ReferenceDocument).MaximumLength(200).WithMessage("Reference document cannot exceed 200 characters.");
        RuleFor(x => x.Notes).MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
    }
}
