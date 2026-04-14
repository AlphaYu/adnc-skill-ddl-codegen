namespace Adnc.Skill.Test.Application.Contracts.Dtos.Customer.Validators;

/// <summary>
/// Validates <see cref="CustomerUpdationDto"/> instances.
/// </summary>
public class CustomerUpdationDtoValidator : AbstractValidator<CustomerUpdationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerUpdationDtoValidator"/> class.
    /// </summary>
    public CustomerUpdationDtoValidator()
    {
        Include(new CustomerCreationDtoValidator());
    }
}
