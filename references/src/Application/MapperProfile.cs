using Adnc.Demo.Admin.Application.Contracts.Dtos.Dict;
using Adnc.Demo.Admin.Application.Contracts.Dtos.Menu;
using Adnc.Demo.Admin.Application.Contracts.Dtos.Organization;
using Adnc.Demo.Admin.Application.Contracts.Dtos.Role;
using Adnc.Demo.Admin.Application.Contracts.Dtos.SysConfig;
using Adnc.Demo.Admin.Application.Contracts.Dtos.User;

namespace Adnc.Demo.Admin.Application;

/// <summary>
/// Configures AutoMapper mappings used by the Admin application layer.
/// </summary>
public sealed class MapperProfile : Profile
{
    /// <summary>
    /// Initializes mapping definitions for entities and DTOs.
    /// </summary>
    public MapperProfile()
    {
        CreateMap(typeof(PagedModel<>), typeof(PageModelDto<>)).ForMember("XData", opt => opt.Ignore());
        CreateMap<MenuCreationDto, Menu>().ForMember(dest => dest.Params, opt => opt.MapFrom<KeyValuesToStringResolver>());
        CreateMap<MenuUpdationDto, Menu>().ForMember(dest => dest.Params, opt => opt.MapFrom<KeyValuesToStringResolver>());
        CreateMap<Menu, MenuDto>().ForMember(dest => dest.Params, opt => opt.MapFrom<StringToKeyValuesResolver>());
        CreateMap<MenuDto, MenuTreeDto>();
        CreateMap<RoleCreationDto, Role>();
        CreateMap<RoleUpdationDto, Role>();
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<RoleMenuRelation, RoleMenuRelationDto>();
        CreateMap<User, UserProfileDto>();
        CreateMap<UserCreationDto, User>();
        CreateMap<UserUpdationDto, User>();
        CreateMap<User, UserDto>();
        CreateMap<OrganizationCreationDto, Organization>();
        CreateMap<OrganizationUpdationDto, Organization>();
        CreateMap<Organization, OrganizationDto>();
        CreateMap<OrganizationDto, OrganizationTreeDto>();
        CreateMap<OrganizationTreeDto, OptionTreeDto>()
            .ForMember(dest => dest.Label, opt => opt.MapFrom(x => x.Name))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(x => x.Id));
        CreateMap<DictCreationDto, Dict>();
        CreateMap<DictUpdationDto, Dict>();
        CreateMap<Dict, DictDto>();
        CreateMap<DictDataCreationDto, DictData>();
        CreateMap<DictDataUpdationDto, DictData>();
        CreateMap<DictData, DictDataDto>();
        CreateMap<SysConfigCreationDto, SysConfig>();
        CreateMap<SysConfigUpdationDto, SysConfig>();
        CreateMap<SysConfig, SysConfigDto>();
    }
}

/// <summary>
/// Converts menu parameter key-value pairs into a serialized string.
/// </summary>
public class KeyValuesToStringResolver : IValueResolver<MenuCreationDto, Menu, string>
{
    /// <summary>
    /// Resolves menu parameters to a serialized string value.
    /// </summary>
    /// <param name="source">The source menu creation DTO.</param>
    /// <param name="destination">The destination menu entity.</param>
    /// <param name="member">The destination member value.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The serialized menu parameter string.</returns>
    public string Resolve(MenuCreationDto source, Menu destination, string member, ResolutionContext context)
    {
        return source.Params.Select(x => $"{x.Key}={x.Value}").ToString("&");
    }
}

/// <summary>
/// Converts a serialized menu parameter string into key-value pairs.
/// </summary>
public class StringToKeyValuesResolver : IValueResolver<Menu, MenuDto, List<KeyValuePair<string, string>>>
{
    /// <summary>
    /// Resolves a serialized menu parameter string into key-value pairs.
    /// </summary>
    /// <param name="source">The source menu entity.</param>
    /// <param name="destination">The destination menu DTO.</param>
    /// <param name="member">The destination member value.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The resolved key-value pairs.</returns>
    public List<KeyValuePair<string, string>> Resolve(Menu source, MenuDto destination, List<KeyValuePair<string, string>> member, ResolutionContext context)
    {
        if (source.Params.IsNullOrEmpty())
        {
            return [];
        }

        var keyValues = new List<KeyValuePair<string, string>>();
        foreach (var item in source.Params.Split("&"))
        {
            var array = item.Split("=");
            keyValues.Add(new KeyValuePair<string, string>(array[0], array[1]));
        }
        return keyValues;
    }
}
