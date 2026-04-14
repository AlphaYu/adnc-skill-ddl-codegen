using System;

namespace Adnc.Skill.Test.Repository.Entities;

/// <summary>
/// Customer
/// </summary>
public class Customer : EfFullAuditEntity, IConcurrency
{
    /// <summary>
    /// name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// phone
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// status
    /// </summary>
    public bool Status { get; set; }

    /// <summary>
    /// rowversion
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
