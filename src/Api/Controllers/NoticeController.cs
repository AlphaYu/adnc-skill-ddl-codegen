using System.Linq;

using Adnc.Skill.Test.Application.Contracts.Dtos.Notice;
using Adnc.Skill.Test.Application.Contracts.Interfaces;

namespace Adnc.Skill.Test.Api.Controllers;

/// <summary>
/// Manages notices.
/// </summary>
[Route($"{RouteConsts.AdminRoot}/notices")]
[ApiController]
public class NoticeController(INoticeService noticeService) : AdncControllerBase
{
    /// <summary>
    /// Creates a notice.
    /// </summary>
    /// <param name="input">The notice to create.</param>
    /// <returns>The ID of the created notice.</returns>
    [HttpPost]
    [AdncAuthorize(PermissionConsts.Notice.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<IdDto>> CreateAsync([FromBody] NoticeCreationDto input)
        => CreatedResult(await noticeService.CreateAsync(input));

    /// <summary>
    /// Updates a notice.
    /// </summary>
    /// <param name="id">The notice ID.</param>
    /// <param name="input">The notice changes.</param>
    /// <returns>A result indicating whether the notice was updated.</returns>
    [HttpPut("{id}")]
    [AdncAuthorize(PermissionConsts.Notice.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<long>> UpdateAsync([FromRoute] long id, [FromBody] NoticeUpdationDto input)
        => Result(await noticeService.UpdateAsync(id, input));

    /// <summary>
    /// Deletes one or more notice records.
    /// </summary>
    /// <param name="ids">The comma-separated notice IDs.</param>
    /// <returns>A result indicating whether the records were deleted.</returns>
    [HttpDelete("{ids}")]
    [AdncAuthorize(PermissionConsts.Notice.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync([FromRoute] string ids)
    {
        var idArr = ids.Split(',').Select(long.Parse).ToArray();
        return Result(await noticeService.DeleteAsync(idArr));
    }

    /// <summary>
    /// Gets a paged list of notices.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of notices.</returns>
    [HttpGet("page")]
    [AdncAuthorize(PermissionConsts.Notice.Search)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PageModelDto<NoticeDto>>> GetPagedAsync([FromQuery] NoticeSearchPagedDto input)
        => await noticeService.GetPagedAsync(input);

    /// <summary>
    /// Gets a notice by ID.
    /// </summary>
    /// <param name="id">The notice ID.</param>
    /// <returns>The requested notice.</returns>
    [HttpGet("{id}")]
    [AdncAuthorize([PermissionConsts.Notice.Get, PermissionConsts.Notice.Update])]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NoticeDto>> GetAsync([FromRoute] long id)
    {
        var notice = await noticeService.GetAsync(id);
        return notice is null ? NotFound() : notice;
    }
}
