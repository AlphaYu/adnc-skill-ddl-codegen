using System.Linq;

using Adnc.Skill.Test.Application.Contracts.Dtos.Tenant;
using Adnc.Skill.Test.Application.Contracts.Interfaces;

namespace Adnc.Skill.Test.Api.Controllers;

/// <summary>
/// Manages tenants.
/// </summary>
[Route($"{RouteConsts.AdminRoot}/tenants")]
[ApiController]
public class TenantController(ITenantService tenantService) : AdncControllerBase
{
    /// <summary>
    /// Creates a tenant.
    /// </summary>
    /// <param name="input">The tenant to create.</param>
    /// <returns>The ID of the created tenant.</returns>
    [HttpPost]
    [AdncAuthorize(PermissionConsts.Tenant.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<IdDto>> CreateAsync([FromBody] TenantCreationDto input)
        => CreatedResult(await tenantService.CreateAsync(input));

    /// <summary>
    /// Updates a tenant.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    /// <param name="input">The tenant changes.</param>
    /// <returns>A result indicating whether the tenant was updated.</returns>
    [HttpPut("{id}")]
    [AdncAuthorize(PermissionConsts.Tenant.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<long>> UpdateAsync([FromRoute] long id, [FromBody] TenantUpdationDto input)
        => Result(await tenantService.UpdateAsync(id, input));

    /// <summary>
    /// Deletes one or more tenant records.
    /// </summary>
    /// <param name="ids">The comma-separated tenant IDs.</param>
    /// <returns>A result indicating whether the records were deleted.</returns>
    [HttpDelete("{ids}")]
    [AdncAuthorize(PermissionConsts.Tenant.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync([FromRoute] string ids)
    {
        var idArr = ids.Split(',').Select(long.Parse).ToArray();
        return Result(await tenantService.DeleteAsync(idArr));
    }

    /// <summary>
    /// Gets a paged list of tenants.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of tenants.</returns>
    [HttpGet("page")]
    [AdncAuthorize(PermissionConsts.Tenant.Search)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PageModelDto<TenantDto>>> GetPagedAsync([FromQuery] TenantSearchPagedDto input)
        => await tenantService.GetPagedAsync(input);

    /// <summary>
    /// Gets a tenant by ID.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    /// <returns>The requested tenant.</returns>
    [HttpGet("{id}")]
    [AdncAuthorize([PermissionConsts.Tenant.Get, PermissionConsts.Tenant.Update])]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantDto>> GetAsync([FromRoute] long id)
    {
        var tenant = await tenantService.GetAsync(id);
        return tenant is null ? NotFound() : tenant;
    }
}
