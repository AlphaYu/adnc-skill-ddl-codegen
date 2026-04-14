# Templates for generated Admin code

Use these templates as starting points. Replace placeholders and then adjust for the specific table.

## Entity

```csharp
namespace {{NamespacePrefix}}.Repository.Entities;

/// <summary>
/// {{EntitySummary}}
/// </summary>
public class {{EntityName}} : EfFullAuditEntity{{ExtraEntityInterfaces}}
{
{{EntityProperties}}
}
```

If the table contains `rowversion`, include:

```csharp
, IConcurrency
```

and keep the concurrency property on the entity:

```csharp
    /// <summary>
    /// {{ColumnCommentOrColumnName}}
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
```

Each generated entity property should follow this doc-comment rule:

```csharp
    /// <summary>
    /// {{ColumnCommentOrColumnName}}
    /// </summary>
    public {{PropertyType}} {{PropertyName}} { get; set; }{{StringInitialization}}
```

Use the DDL column `COMMENT` text verbatim for `{{ColumnCommentOrColumnName}}`. If that comment is empty or absent, use the original SQL column name instead.

If the table is soft-delete enabled:

```csharp
, ISoftDelete
```

and include:

```csharp
    /// <summary>
    /// Deletion flag.
    /// </summary>
    public bool IsDeleted { get; set; }
```

## Entity config

```csharp
namespace {{NamespacePrefix}}.Repository.Entities.Config;

public class {{EntityName}}Config : AbstractEntityTypeConfiguration<{{EntityName}}>
{
    public override void Configure(EntityTypeBuilder<{{EntityName}}> builder)
    {
        base.Configure(builder);

{{PropertyMaxLengthMappings}}
    }
}
```

## Service interface

```csharp
using {{NamespacePrefix}}.Application.Contracts.Dtos.{{EntityName}};

namespace {{NamespacePrefix}}.Application.Contracts.Interfaces;

/// <summary>
/// Defines {{entity summary lowercase}} management services.
/// </summary>
public interface I{{EntityName}}Service : IAppService
{
    /// <summary>
    /// Creates a {{entity summary lowercase}}.
    /// </summary>
    /// <param name="input">The {{entity summary lowercase}} to create.</param>
    /// <returns>The ID of the created {{entity summary lowercase}}.</returns>
    [OperateLog(LogName = "Create {{entity summary lowercase}}")]
    Task<ServiceResult<IdDto>> CreateAsync({{EntityName}}CreationDto input);

    /// <summary>
    /// Updates a {{entity summary lowercase}}.
    /// </summary>
    /// <param name="id">The {{entity summary lowercase}} ID.</param>
    /// <param name="input">The {{entity summary lowercase}} changes.</param>
    /// <returns>A result indicating whether the {{entity summary lowercase}} was updated.</returns>
    [OperateLog(LogName = "Update {{entity summary lowercase}}")]
    Task<ServiceResult> UpdateAsync(long id, {{EntityName}}UpdationDto input);

    /// <summary>
    /// Deletes one or more {{entity summary lowercase}} records.
    /// </summary>
    /// <param name="ids">The {{entity summary lowercase}} IDs.</param>
    /// <returns>A result indicating whether the records were deleted.</returns>
    [OperateLog(LogName = "Delete {{entity summary lowercase}}")]
    Task<ServiceResult> DeleteAsync(long[] ids);

    /// <summary>
    /// Gets a {{entity summary lowercase}} by ID.
    /// </summary>
    /// <param name="id">The {{entity summary lowercase}} ID.</param>
    /// <returns>The requested {{entity summary lowercase}}, or <c>null</c> if it does not exist.</returns>
    Task<{{EntityName}}Dto?> GetAsync(long id);

    /// <summary>
    /// Gets a paged list of {{entity summary lowercase}} records.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of {{entity summary lowercase}} records.</returns>
    Task<PageModelDto<{{EntityName}}Dto>> GetPagedAsync({{EntityName}}SearchPagedDto input);
}
```

## Service

```csharp
namespace {{NamespacePrefix}}.Application.Services;

/// <inheritdoc cref="I{{EntityName}}Service"/>
public class {{EntityName}}Service(IEfRepository<{{EntityName}}> {{entityVar}}Repo)
    : AbstractAppService, I{{EntityName}}Service
{
    /// <inheritdoc />
    public async Task<ServiceResult<IdDto>> CreateAsync({{EntityName}}CreationDto input)
    {
        input.TrimStringFields();
{{CreateUniqueChecks}}
        var entity = Mapper.Map<{{EntityName}}>(input, IdGenerater.GetNextId());
        await {{entityVar}}Repo.InsertAsync(entity);
        return new IdDto(entity.Id);
    }

    /// <inheritdoc />
    public async Task<ServiceResult> UpdateAsync(long id, {{EntityName}}UpdationDto input)
    {
        input.TrimStringFields();
        var entity = await {{entityVar}}Repo.FetchAsync(x => x.Id == id, noTracking: false);
        if (entity is null)
        {
            return Problem(HttpStatusCode.NotFound, "This {{entity summary lowercase}} does not exist");
        }
{{UpdateUniqueChecks}}
        var newEntity = Mapper.Map(input, entity);
        await {{entityVar}}Repo.UpdateAsync(newEntity);
        return ServiceResult();
    }

    /// <inheritdoc />
    public async Task<ServiceResult> DeleteAsync(long[] ids)
    {
        await {{entityVar}}Repo.ExecuteDeleteAsync(x => ids.Contains(x.Id));
        return ServiceResult();
    }

    /// <inheritdoc />
    public async Task<{{EntityName}}Dto?> GetAsync(long id)
    {
        var entity = await {{entityVar}}Repo.FetchAsync(x => x.Id == id);
        return entity is null ? null : Mapper.Map<{{EntityName}}Dto>(entity);
    }

    /// <inheritdoc />
    public async Task<PageModelDto<{{EntityName}}Dto>> GetPagedAsync({{EntityName}}SearchPagedDto input)
    {
        input.TrimStringFields();
        var whereExpr = ExpressionCreator
            .New<{{EntityName}}>()
{{WhereConditions}}
        var total = await {{entityVar}}Repo.CountAsync(whereExpr);
        if (total == 0)
        {
            return new PageModelDto<{{EntityName}}Dto>(input);
        }

        var entities = await {{entityVar}}Repo
                                        .Where(whereExpr)
                                        .OrderByDescending(x => x.Id)
                                        .Skip(input.SkipRows())
                                        .Take(input.PageSize)
                                        .ToListAsync();
        var dtos = Mapper.Map<List<{{EntityName}}Dto>>(entities);
        return new PageModelDto<{{EntityName}}Dto>(input, dtos, total);
    }
}
```

## DTOs

Creation DTO:

```csharp
namespace {{NamespacePrefix}}.Application.Contracts.Dtos.{{EntityName}};

/// <summary>
/// Represents the payload used to create a {{entity summary lowercase}}.
/// </summary>
public class {{EntityName}}CreationDto : InputDto
{
{{CreationDtoProperties}}
}
```

Updation DTO:

```csharp
namespace {{NamespacePrefix}}.Application.Contracts.Dtos.{{EntityName}};

/// <summary>
/// Represents the payload used to update a {{entity summary lowercase}}.
/// </summary>
public class {{EntityName}}UpdationDto : {{EntityName}}CreationDto
{ }
```

Search DTO:

```csharp
namespace {{NamespacePrefix}}.Application.Contracts.Dtos.{{EntityName}};

/// <summary>
/// Represents the paging and filtering criteria used to search {{entity summary lowercase}} records.
/// </summary>
public class {{EntityName}}SearchPagedDto : SearchPagedDto
{
{{SearchDtoProperties}}
}
```

Read DTO:

```csharp
namespace {{NamespacePrefix}}.Application.Contracts.Dtos.{{EntityName}};

/// <summary>
/// Represents a {{entity summary lowercase}}.
/// </summary>
[Serializable]
public class {{EntityName}}Dto : {{EntityName}}CreationDto
{
    /// <summary>
    /// Gets or sets the {{entity summary lowercase}} ID.
    /// </summary>
    public long Id { get; set; }
}
```

## Validators

Creation validator:

```csharp
namespace {{NamespacePrefix}}.Application.Contracts.Dtos.{{EntityName}}.Validators;

/// <summary>
/// Validates <see cref="{{EntityName}}CreationDto"/> instances.
/// </summary>
public class {{EntityName}}CreationDtoValidator : AbstractValidator<{{EntityName}}CreationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="{{EntityName}}CreationDtoValidator"/> class.
    /// </summary>
    public {{EntityName}}CreationDtoValidator()
    {
{{ValidationRules}}
    }
}
```

Updation validator:

```csharp
namespace {{NamespacePrefix}}.Application.Contracts.Dtos.{{EntityName}}.Validators;

/// <summary>
/// Validates <see cref="{{EntityName}}UpdationDto"/> instances.
/// </summary>
public class {{EntityName}}UpdationDtoValidator : AbstractValidator<{{EntityName}}UpdationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="{{EntityName}}UpdationDtoValidator"/> class.
    /// </summary>
    public {{EntityName}}UpdationDtoValidator()
    {
        Include(new {{EntityName}}CreationDtoValidator());
    }
}
```

## Controller

```csharp
using {{NamespacePrefix}}.Application.Contracts.Dtos.{{EntityName}};

namespace {{NamespacePrefix}}.Api.Controllers;

/// <summary>
/// Manages {{entity summary lowercase}} records.
/// </summary>
[Route($"{RouteConsts.AdminRoot}/{{route-segment}}")]
[ApiController]
public class {{EntityName}}Controller(I{{EntityName}}Service {{entityVar}}Service) : AdncControllerBase
{
    /// <summary>
    /// Creates a {{entity summary lowercase}}.
    /// </summary>
    /// <param name="input">The {{entity summary lowercase}} to create.</param>
    /// <returns>The ID of the created {{entity summary lowercase}}.</returns>
    [HttpPost]
    [AdncAuthorize(PermissionConsts.{{EntityName}}.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<IdDto>> CreateAsync([FromBody] {{EntityName}}CreationDto input)
        => CreatedResult(await {{entityVar}}Service.CreateAsync(input));

    /// <summary>
    /// Updates a {{entity summary lowercase}}.
    /// </summary>
    /// <param name="id">The {{entity summary lowercase}} ID.</param>
    /// <param name="input">The {{entity summary lowercase}} changes.</param>
    /// <returns>A result indicating whether the {{entity summary lowercase}} was updated.</returns>
    [HttpPut("{id}")]
    [AdncAuthorize(PermissionConsts.{{EntityName}}.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<long>> UpdateAsync([FromRoute] long id, [FromBody] {{EntityName}}UpdationDto input)
        => Result(await {{entityVar}}Service.UpdateAsync(id, input));

    /// <summary>
    /// Deletes one or more {{entity summary lowercase}} records.
    /// </summary>
    /// <param name="ids">The comma-separated {{entity summary lowercase}} IDs.</param>
    /// <returns>A result indicating whether the records were deleted.</returns>
    [HttpDelete("{ids}")]
    [AdncAuthorize(PermissionConsts.{{EntityName}}.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync([FromRoute] string ids)
    {
        var idArr = ids.Split(',').Select(long.Parse).ToArray();
        return Result(await {{entityVar}}Service.DeleteAsync(idArr));
    }

    /// <summary>
    /// Gets a paged list of {{entity summary lowercase}} records.
    /// </summary>
    /// <param name="input">The paging and filtering criteria.</param>
    /// <returns>A paged list of {{entity summary lowercase}} records.</returns>
    [HttpGet("page")]
    [AdncAuthorize(PermissionConsts.{{EntityName}}.Search)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PageModelDto<{{EntityName}}Dto>>> GetPagedAsync([FromQuery] {{EntityName}}SearchPagedDto input)
        => await {{entityVar}}Service.GetPagedAsync(input);

    /// <summary>
    /// Gets a {{entity summary lowercase}} by ID.
    /// </summary>
    /// <param name="id">The {{entity summary lowercase}} ID.</param>
    /// <returns>The requested {{entity summary lowercase}}.</returns>
    [HttpGet("{id}")]
    [AdncAuthorize([PermissionConsts.{{EntityName}}.Get, PermissionConsts.{{EntityName}}.Update])]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<{{EntityName}}Dto>> GetAsync([FromRoute] long id)
    {
        var dto = await {{entityVar}}Service.GetAsync(id);
        return dto is null ? NotFound() : dto;
    }
}
```

## Shared file updates

EntityInfo:

```csharp
modelBuilder.Entity<{{EntityName}}>().ToTable("{{table_name}}");
```

MapperProfile:

```csharp
CreateMap<{{EntityName}}CreationDto, {{EntityName}}>();
CreateMap<{{EntityName}}UpdationDto, {{EntityName}}>();
CreateMap<{{EntityName}}, {{EntityName}}Dto>();
```

PermissionConsts:

```csharp
public static class {{EntityName}}
{
    public const string Create = "{{permission-prefix}}-create";
    public const string Update = "{{permission-prefix}}-update";
    public const string Delete = "{{permission-prefix}}-delete";
    public const string Search = "{{permission-prefix}}-search";
    public const string Get = "{{permission-prefix}}-get";
}
```
