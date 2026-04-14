using System.Linq;

using Adnc.Skill.Test.Application.Contracts.Dtos.Customer;
using Adnc.Skill.Test.Application.Contracts.Interfaces;

namespace Adnc.Skill.Test.Api.Controllers;

/// <summary>
/// Manages customers.
/// </summary>
[Route($"{RouteConsts.AdminRoot}/customers")]
[ApiController]
public class CustomerController(ICustomerService customerService) : AdncControllerBase
{
    /// <summary>
    /// Creates a customer.
    /// </summary>
    /// <param name="input">The customer to create.</param>
    /// <returns>The ID of the created customer.</returns>
    [HttpPost]
    [AdncAuthorize(PermissionConsts.Customer.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<IdDto>> CreateAsync([FromBody] CustomerCreationDto input)
        => CreatedResult(await customerService.CreateAsync(input));

    /// <summary>
    /// Updates a customer.
    /// </summary>
    /// <param name="id">The customer ID.</param>
    /// <param name="input">The customer changes.</param>
    /// <returns>A result indicating whether the customer was updated.</returns>
    [HttpPut("{id}")]
    [AdncAuthorize(PermissionConsts.Customer.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<long>> UpdateAsync([FromRoute] long id, [FromBody] CustomerUpdationDto input)
        => Result(await customerService.UpdateAsync(id, input));

    /// <summary>
    /// Deletes one or more customer records.
    /// </summary>
    /// <param name="ids">The comma-separated customer IDs.</param>
    /// <returns>A result indicating whether the records were deleted.</returns>
    [HttpDelete("{ids}")]
    [AdncAuthorize(PermissionConsts.Customer.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync([FromRoute] string ids)
    {
        var idArr = ids.Split(',').Select(long.Parse).ToArray();
        return Result(await customerService.DeleteAsync(idArr));
    }

    /// <summary>
    /// Gets a paged list of customers.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of customers.</returns>
    [HttpGet("page")]
    [AdncAuthorize(PermissionConsts.Customer.Search)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PageModelDto<CustomerDto>>> GetPagedAsync([FromQuery] CustomerSearchPagedDto input)
        => await customerService.GetPagedAsync(input);

    /// <summary>
    /// Gets a customer by ID.
    /// </summary>
    /// <param name="id">The customer ID.</param>
    /// <returns>The requested customer.</returns>
    [HttpGet("{id}")]
    [AdncAuthorize([PermissionConsts.Customer.Get, PermissionConsts.Customer.Update])]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetAsync([FromRoute] long id)
    {
        var customer = await customerService.GetAsync(id);
        return customer is null ? NotFound() : customer;
    }
}
