using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.EntityFrameworkCore;

using Adnc.Skill.Test.Application.Contracts.Dtos.Tenant;
using Adnc.Skill.Test.Application.Contracts.Interfaces;
using Adnc.Skill.Test.Repository.Entities;

namespace Adnc.Skill.Test.Application.Services;

/// <inheritdoc cref="ITenantService"/>
public class TenantService(IEfRepository<Tenant> tenantRepo)
    : AbstractAppService, ITenantService
{
    /// <inheritdoc />
    public async Task<ServiceResult<IdDto>> CreateAsync(TenantCreationDto input)
    {
        input.TrimStringFields();

        var codeExists = await tenantRepo.AnyAsync(x => x.Code == input.Code);
        if (codeExists)
        {
            return Problem(HttpStatusCode.BadRequest, "This tenant code already exists");
        }

        var entity = Mapper.Map<Tenant>(input, IdGenerater.GetNextId());
        await tenantRepo.InsertAsync(entity);
        return new IdDto(entity.Id);
    }

    /// <inheritdoc />
    public async Task<ServiceResult> UpdateAsync(long id, TenantUpdationDto input)
    {
        input.TrimStringFields();
        var entity = await tenantRepo.FetchAsync(x => x.Id == id, noTracking: false);
        if (entity is null)
        {
            return Problem(HttpStatusCode.NotFound, "This tenant does not exist");
        }

        var codeExists = await tenantRepo.AnyAsync(x => x.Code == input.Code && x.Id != id);
        if (codeExists)
        {
            return Problem(HttpStatusCode.BadRequest, "This tenant code already exists");
        }

        var newEntity = Mapper.Map(input, entity);
        await tenantRepo.UpdateAsync(newEntity);
        return ServiceResult();
    }

    /// <inheritdoc />
    public async Task<ServiceResult> DeleteAsync(long[] ids)
    {
        await tenantRepo.ExecuteDeleteAsync(x => ids.Contains(x.Id));
        return ServiceResult();
    }

    /// <inheritdoc />
    public async Task<TenantDto?> GetAsync(long id)
    {
        var entity = await tenantRepo.FetchAsync(x => x.Id == id);
        return entity is null ? null : Mapper.Map<TenantDto>(entity);
    }

    /// <inheritdoc />
    public async Task<PageModelDto<TenantDto>> GetPagedAsync(TenantSearchPagedDto input)
    {
        input.TrimStringFields();
        var whereExpr = ExpressionCreator
            .New<Tenant>()
            .AndIf(input.Keywords.IsNotNullOrWhiteSpace(), x => EF.Functions.Like(x.Name, $"{input.Keywords}%") || EF.Functions.Like(x.Code, $"{input.Keywords}%"));

        var total = await tenantRepo.CountAsync(whereExpr);
        if (total == 0)
        {
            return new PageModelDto<TenantDto>(input);
        }

        var entities = await tenantRepo
                                      .Where(whereExpr)
                                      .OrderByDescending(x => x.Id)
                                      .Skip(input.SkipRows())
                                      .Take(input.PageSize)
                                      .ToListAsync();
        var dtos = Mapper.Map<List<TenantDto>>(entities);
        return new PageModelDto<TenantDto>(input, dtos, total);
    }
}
