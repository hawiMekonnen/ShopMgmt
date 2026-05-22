using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Validators;

namespace ShopMgmt.Tests.Validators;

public class CreateMaterialDtoValidatorTests
{
    [Fact]
    public void EmptyName_FailsValidation()
    {
        var validator = new CreateMaterialDtoValidator();
        var result = validator.Validate(new CreateMaterialDto
        {
            PartNumber = "PN-1",
            Name = "",
            CategoryId = 1,
            UnitPrice = 1m,
            Unit = "ea"
        });

        Assert.False(result.IsValid);
    }
}
