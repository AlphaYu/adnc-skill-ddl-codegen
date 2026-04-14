using Adnc.Skill.Test.Application.Contracts.Dtos.Customer;

namespace Adnc.Skill.Test.Application.Contracts.Interfaces;

/// <summary>
/// Defines customer management services.
/// </summary>
public interface ICustomerService : IAppService
{
    /// <summary>
    /// Creates a customer.
    /// </summary>
    /// <param name="input">The customer to create.</param>
    /// <returns>The ID of the created customer.</returns>
    [OperateLog(LogName = "Create customer")]
    Task<ServiceResult<IdDto>> CreateAsync(CustomerCreationDto input);

    /// <summary>
    /// Updates a customer.
    /// </summary>
    /// <param name="id">The customer ID.</param>
    /// <param name="input">The customer changes.</param>
    /// <returns>A result indicating whether the customer was updated.</returns>
    [OperateLog(LogName = "Update customer")]
    Task<ServiceResult> UpdateAsync(long id, CustomerUpdationDto input);

    /// <summary>
    /// Deletes one or more customer records.
    /// </summary>
    /// <param name="ids">The customer IDs.</param>
    /// <returns>A result indicating whether the records were deleted.</returns>
    [OperateLog(LogName = "Delete customer")]
    Task<ServiceResult> DeleteAsync(long[] ids);

    /// <summary>
    /// Gets a customer by ID.
    /// </summary>
    /// <param name="id">The customer ID.</param>
    /// <returns>The requested customer, or <c>null</c> if it does not exist.</returns>
    Task<CustomerDto?> GetAsync(long id);

    /// <summary>
    /// Gets a paged list of customers.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of customers.</returns>
    Task<PageModelDto<CustomerDto>> GetPagedAsync(CustomerSearchPagedDto input);
}
