using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.EntityFrameworkCore;

using Adnc.Skill.Test.Application.Contracts.Dtos.Notice;
using Adnc.Skill.Test.Application.Contracts.Interfaces;
using Adnc.Skill.Test.Repository.Entities;

namespace Adnc.Skill.Test.Application.Services;

/// <inheritdoc cref="INoticeService"/>
public class NoticeService(IEfRepository<Notice> noticeRepo)
    : AbstractAppService, INoticeService
{
    /// <inheritdoc />
    public async Task<ServiceResult<IdDto>> CreateAsync(NoticeCreationDto input)
    {
        input.TrimStringFields();
        var entity = Mapper.Map<Notice>(input, IdGenerater.GetNextId());
        await noticeRepo.InsertAsync(entity);
        return new IdDto(entity.Id);
    }

    /// <inheritdoc />
    public async Task<ServiceResult> UpdateAsync(long id, NoticeUpdationDto input)
    {
        input.TrimStringFields();
        var entity = await noticeRepo.FetchAsync(x => x.Id == id, noTracking: false);
        if (entity is null)
        {
            return Problem(HttpStatusCode.NotFound, "This notice does not exist");
        }

        var newEntity = Mapper.Map(input, entity);
        await noticeRepo.UpdateAsync(newEntity);
        return ServiceResult();
    }

    /// <inheritdoc />
    public async Task<ServiceResult> DeleteAsync(long[] ids)
    {
        await noticeRepo.ExecuteDeleteAsync(x => ids.Contains(x.Id));
        return ServiceResult();
    }

    /// <inheritdoc />
    public async Task<NoticeDto?> GetAsync(long id)
    {
        var entity = await noticeRepo.FetchAsync(x => x.Id == id);
        return entity is null ? null : Mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<PageModelDto<NoticeDto>> GetPagedAsync(NoticeSearchPagedDto input)
    {
        input.TrimStringFields();
        var whereExpr = ExpressionCreator
            .New<Notice>()
            .AndIf(input.Keywords.IsNotNullOrWhiteSpace(), x => EF.Functions.Like(x.Title, $"{input.Keywords}%") || EF.Functions.Like(x.Content, $"{input.Keywords}%"));

        var total = await noticeRepo.CountAsync(whereExpr);
        if (total == 0)
        {
            return new PageModelDto<NoticeDto>(input);
        }

        var entities = await noticeRepo
                                      .Where(whereExpr)
                                      .OrderByDescending(x => x.Id)
                                      .Skip(input.SkipRows())
                                      .Take(input.PageSize)
                                      .ToListAsync();
        var dtos = Mapper.Map<List<NoticeDto>>(entities);
        return new PageModelDto<NoticeDto>(input, dtos, total);
    }
}
