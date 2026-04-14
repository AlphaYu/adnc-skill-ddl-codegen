namespace Adnc.Skill.Test.Application.Contracts.Dtos.Notice.Validators;

/// <summary>
/// Validates <see cref="NoticeUpdationDto"/> instances.
/// </summary>
public class NoticeUpdationDtoValidator : AbstractValidator<NoticeUpdationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoticeUpdationDtoValidator"/> class.
    /// </summary>
    public NoticeUpdationDtoValidator()
    {
        Include(new NoticeCreationDtoValidator());
    }
}
