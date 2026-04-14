using System;

namespace Adnc.Skill.Test.Application.Contracts.Dtos.Notice;

/// <summary>
/// Represents a notice.
/// </summary>
[Serializable]
public class NoticeDto : NoticeCreationDto
{
    /// <summary>
    /// Gets or sets the notice ID.
    /// </summary>
    public long Id { get; set; }
}
