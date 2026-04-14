using Adnc.Skill.Test.Application.Contracts.Dtos.Tenant;

namespace Adnc.Skill.Test.Application.Contracts.Interfaces;

/// <summary>
/// Defines tenant management services.
/// </summary>
public interface ITenantService : IAppService
{
    /// <summary>
    /// Creates a tenant.
    /// </summary>
    /// <param name="input">The tenant to create.</param>
    /// <returns>The ID of the created tenant.</returns>
    [OperateLog(LogName = "Create tenant")]
    Task<ServiceResult<IdDto>> CreateAsync(TenantCreationDto input);

    /// <summary>
    /// Updates a tenant.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    /// <param name="input">The tenant changes.</param>
    /// <returns>A result indicating whether the tenant was updated.</returns>
    [OperateLog(LogName = "Update tenant")]
    Task<ServiceResult> UpdateAsync(long id, TenantUpdationDto input);

    /// <summary>
    /// Deletes one or more tenant records.
    /// </summary>
    /// <param name="ids">The tenant IDs.</param>
    /// <returns>A result indicating whether the records were deleted.</returns>
    [OperateLog(LogName = "Delete tenant")]
    Task<ServiceResult> DeleteAsync(long[] ids);

    /// <summary>
    /// Gets a tenant by ID.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    /// <returns>The requested tenant, or <c>null</c> if it does not exist.</returns>
    Task<TenantDto?> GetAsync(long id);

    /// <summary>
    /// Gets a paged list of tenants.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of tenants.</returns>
    Task<PageModelDto<TenantDto>> GetPagedAsync(TenantSearchPagedDto input);
}
