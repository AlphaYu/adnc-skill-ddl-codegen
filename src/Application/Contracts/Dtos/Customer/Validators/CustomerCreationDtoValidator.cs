using Adnc.Skill.Test.Repository;

namespace Adnc.Skill.Test.Application.Contracts.Dtos.Customer.Validators;

/// <summary>
/// Validates <see cref="CustomerCreationDto"/> instances.
/// </summary>
public class CustomerCreationDtoValidator : AbstractValidator<CustomerCreationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerCreationDtoValidator"/> class.
    /// </summary>
    public CustomerCreationDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(CustomerConsts.Name_MaxLength);
        RuleFor(x => x.Phone).MaximumLength(CustomerConsts.Phone_MaxLength);
    }
}
