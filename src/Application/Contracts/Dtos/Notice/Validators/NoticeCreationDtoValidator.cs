using Adnc.Skill.Test.Repository;

namespace Adnc.Skill.Test.Application.Contracts.Dtos.Notice.Validators;

/// <summary>
/// Validates <see cref="NoticeCreationDto"/> instances.
/// </summary>
public class NoticeCreationDtoValidator : AbstractValidator<NoticeCreationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoticeCreationDtoValidator"/> class.
    /// </summary>
    public NoticeCreationDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(NoticeConsts.Title_MaxLength);
        RuleFor(x => x.Content).MaximumLength(NoticeConsts.Content_MaxLength);
    }
}
