namespace Adnc.Skill.Test.Application.Contracts.Dtos.Tenant;

/// <summary>
/// Represents the payload used to create a tenant.
/// </summary>
public class TenantCreationDto : InputDto
{
    /// <summary>
    /// Gets or sets the tenant code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the tenant is enabled.
    /// </summary>
    public bool Status { get; set; }
}
