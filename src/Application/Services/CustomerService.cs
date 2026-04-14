using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.EntityFrameworkCore;

using Adnc.Skill.Test.Application.Contracts.Dtos.Customer;
using Adnc.Skill.Test.Application.Contracts.Interfaces;
using Adnc.Skill.Test.Repository.Entities;

namespace Adnc.Skill.Test.Application.Services;

/// <inheritdoc cref="ICustomerService"/>
public class CustomerService(IEfRepository<Customer> customerRepo)
    : AbstractAppService, ICustomerService
{
    /// <inheritdoc />
    public async Task<ServiceResult<IdDto>> CreateAsync(CustomerCreationDto input)
    {
        input.TrimStringFields();
        var entity = Mapper.Map<Customer>(input, IdGenerater.GetNextId());
        await customerRepo.InsertAsync(entity);
        return new IdDto(entity.Id);
    }

    /// <inheritdoc />
    public async Task<ServiceResult> UpdateAsync(long id, CustomerUpdationDto input)
    {
        input.TrimStringFields();
        var entity = await customerRepo.FetchAsync(x => x.Id == id, noTracking: false);
        if (entity is null)
        {
            return Problem(HttpStatusCode.NotFound, "This customer does not exist");
        }

        var newEntity = Mapper.Map(input, entity);
        await customerRepo.UpdateAsync(newEntity);
        return ServiceResult();
    }

    /// <inheritdoc />
    public async Task<ServiceResult> DeleteAsync(long[] ids)
    {
        await customerRepo.ExecuteDeleteAsync(x => ids.Contains(x.Id));
        return ServiceResult();
    }

    /// <inheritdoc />
    public async Task<CustomerDto?> GetAsync(long id)
    {
        var entity = await customerRepo.FetchAsync(x => x.Id == id);
        return entity is null ? null : Mapper.Map<CustomerDto>(entity);
    }

    /// <inheritdoc />
    public async Task<PageModelDto<CustomerDto>> GetPagedAsync(CustomerSearchPagedDto input)
    {
        input.TrimStringFields();
        var whereExpr = ExpressionCreator
            .New<Customer>()
            .AndIf(input.Keywords.IsNotNullOrWhiteSpace(), x => EF.Functions.Like(x.Name, $"{input.Keywords}%") || EF.Functions.Like(x.Phone, $"{input.Keywords}%"));

        var total = await customerRepo.CountAsync(whereExpr);
        if (total == 0)
        {
            return new PageModelDto<CustomerDto>(input);
        }

        var entities = await customerRepo
                                        .Where(whereExpr)
                                        .OrderByDescending(x => x.Id)
                                        .Skip(input.SkipRows())
                                        .Take(input.PageSize)
                                        .ToListAsync();
        var dtos = Mapper.Map<List<CustomerDto>>(entities);
        return new PageModelDto<CustomerDto>(input, dtos, total);
    }
}
