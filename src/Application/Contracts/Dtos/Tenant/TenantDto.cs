using System;

namespace Adnc.Skill.Test.Application.Contracts.Dtos.Tenant;

/// <summary>
/// Represents a tenant.
/// </summary>
[Serializable]
public class TenantDto : TenantCreationDto
{
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public long Id { get; set; }
}
