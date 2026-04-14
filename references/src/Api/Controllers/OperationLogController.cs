using Adnc.Demo.Admin.Application.Contracts.Dtos.Log;

namespace Adnc.Demo.Admin.Api.Controllers;

/// <summary>
/// Manages operation log queries.
/// </summary>
[Route($"{RouteConsts.MaintRoot}/operationlogs")]
public class OperationLogController(ILogService logService, UserContext userContext) : AdncControllerBase
{
    /// <summary>
    /// Gets a paged list of operation logs.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of operation logs.</returns>
    [HttpGet("page")]
    [AdncAuthorize(PermissionConsts.Log.SearchForOperationLog)]
    public async Task<ActionResult<PageModelDto<OperationLogDto>>> GetOpsLogsPaged([FromQuery] SearchPagedDto input)
        => await logService.GetOperationLogsPagedAsync(input);

    /// <summary>
    /// Gets a paged list of operation logs for the current user.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of operation logs for the current user.</returns>
    [HttpGet("user/page")]
    public async Task<ActionResult<PageModelDto<OperationLogDto>>> GetUserOpsLogsPagedAsync([FromQuery] SearchPagedDto input)
    {
        input.Keywords = userContext.Account;
        return await logService.GetOperationLogsPagedAsync(input);
    }
}
