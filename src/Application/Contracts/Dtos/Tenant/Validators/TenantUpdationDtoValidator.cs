namespace Adnc.Skill.Test.Application.Contracts.Dtos.Tenant.Validators;

/// <summary>
/// Validates <see cref="TenantUpdationDto"/> instances.
/// </summary>
public class TenantUpdationDtoValidator : AbstractValidator<TenantUpdationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantUpdationDtoValidator"/> class.
    /// </summary>
    public TenantUpdationDtoValidator()
    {
        Include(new TenantCreationDtoValidator());
    }
}
