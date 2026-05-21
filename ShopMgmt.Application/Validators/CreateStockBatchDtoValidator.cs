using FluentValidation;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Validators;

public class CreateStockBatchDtoValidator : AbstractValidator<CreateStockBatchDto>
{
    public CreateStockBatchDtoValidator()
    {
        RuleFor(x => x.QuantityReceived).GreaterThan(0);
        RuleFor(x => x.CostTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReceivedAt).NotEmpty();
    }
}
