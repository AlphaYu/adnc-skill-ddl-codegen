using System;

namespace Adnc.Skill.Test.Repository.Entities;

/// <summary>
/// Notice
/// </summary>
public class Notice : EfFullAuditEntity, ISoftDelete, IConcurrency
{
    /// <summary>
    /// title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// isdeleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// rowversion
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
