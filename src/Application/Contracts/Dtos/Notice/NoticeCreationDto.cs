namespace Adnc.Skill.Test.Application.Contracts.Dtos.Notice;

/// <summary>
/// Represents the payload used to create a notice.
/// </summary>
public class NoticeCreationDto : InputDto
{
    /// <summary>
    /// Gets or sets the notice title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notice content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
