using Adnc.Skill.Test.Application.Contracts.Dtos.Notice;

namespace Adnc.Skill.Test.Application.Contracts.Interfaces;

/// <summary>
/// Defines notice management services.
/// </summary>
public interface INoticeService : IAppService
{
    /// <summary>
    /// Creates a notice.
    /// </summary>
    /// <param name="input">The notice to create.</param>
    /// <returns>The ID of the created notice.</returns>
    [OperateLog(LogName = "Create notice")]
    Task<ServiceResult<IdDto>> CreateAsync(NoticeCreationDto input);

    /// <summary>
    /// Updates a notice.
    /// </summary>
    /// <param name="id">The notice ID.</param>
    /// <param name="input">The notice changes.</param>
    /// <returns>A result indicating whether the notice was updated.</returns>
    [OperateLog(LogName = "Update notice")]
    Task<ServiceResult> UpdateAsync(long id, NoticeUpdationDto input);

    /// <summary>
    /// Deletes one or more notice records.
    /// </summary>
    /// <param name="ids">The notice IDs.</param>
    /// <returns>A result indicating whether the records were deleted.</returns>
    [OperateLog(LogName = "Delete notice")]
    Task<ServiceResult> DeleteAsync(long[] ids);

    /// <summary>
    /// Gets a notice by ID.
    /// </summary>
    /// <param name="id">The notice ID.</param>
    /// <returns>The requested notice, or <c>null</c> if it does not exist.</returns>
    Task<NoticeDto?> GetAsync(long id);

    /// <summary>
    /// Gets a paged list of notices.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of notices.</returns>
    Task<PageModelDto<NoticeDto>> GetPagedAsync(NoticeSearchPagedDto input);
}
