using Adnc.Skill.Test.Repository;

namespace Adnc.Skill.Test.Application.Contracts.Dtos.Tenant.Validators;

/// <summary>
/// Validates <see cref="TenantCreationDto"/> instances.
/// </summary>
public class TenantCreationDtoValidator : AbstractValidator<TenantCreationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantCreationDtoValidator"/> class.
    /// </summary>
    public TenantCreationDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(TenantConsts.Code_MaxLength);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(TenantConsts.Name_MaxLength);
    }
}
