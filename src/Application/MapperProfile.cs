using AutoMapper;

using Adnc.Skill.Test.Application.Contracts.Dtos.Customer;
using Adnc.Skill.Test.Application.Contracts.Dtos.Notice;
using Adnc.Skill.Test.Application.Contracts.Dtos.Tenant;
using Adnc.Skill.Test.Repository.Entities;

namespace Adnc.Skill.Test.Application;

/// <summary>
/// Configures AutoMapper mappings used by the application layer.
/// </summary>
public sealed class MapperProfile : Profile
{
    /// <summary>
    /// Initializes mapping definitions for entities and DTOs.
    /// </summary>
    public MapperProfile()
    {
        CreateMap(typeof(PagedModel<>), typeof(PageModelDto<>)).ForMember("XData", opt => opt.Ignore());

        CreateMap<CustomerCreationDto, Customer>();
        CreateMap<CustomerUpdationDto, Customer>();
        CreateMap<Customer, CustomerDto>();

        CreateMap<NoticeCreationDto, Notice>();
        CreateMap<NoticeUpdationDto, Notice>();
        CreateMap<Notice, NoticeDto>();

        CreateMap<TenantCreationDto, Tenant>();
        CreateMap<TenantUpdationDto, Tenant>();
        CreateMap<Tenant, TenantDto>();
    }
}
