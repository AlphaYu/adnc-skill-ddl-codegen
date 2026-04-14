using System;

namespace Adnc.Skill.Test.Repository.Entities;

/// <summary>
/// Tenant
/// </summary>
public class Tenant : EfFullAuditEntity, IConcurrency
{
    /// <summary>
    /// code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// status
    /// </summary>
    public bool Status { get; set; }

    /// <summary>
    /// rowversion
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
