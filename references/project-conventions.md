# Project conventions for Adnc code generation

Read these files before generating new code:

- `references\.editorconfig` (copied from `src\.editorconfig`)
- `references\src\Repository\EntityInfo.cs`
- `references\src\Repository\EntityConsts.cs`
- `references\src\Repository\Entities\Dict.cs`
- `references\src\Repository\Entities\Menu.cs`
- `references\src\Repository\Entities\Organization.cs`
- `references\src\Repository\Entities\User.cs`
- `references\src\Repository\Entities\Config\DictConfig.cs`
- `references\src\Repository\Entities\Config\UserConfig.cs`
- `references\src\Application\MapperProfile.cs`
- `references\src\Application\Contracts\Interfaces\IDictService.cs`
- `references\src\Application\Contracts\Interfaces\ISysConfigService.cs`
- `references\src\Application\Services\DictService.cs`
- `references\src\Application\Services\SysConfigService.cs`
- `references\src\Application\Contracts\Dtos\Dict\DictCreationDto.cs`
- `references\src\Application\Contracts\Dtos\Dict\DictUpdationDto.cs`
- `references\src\Application\Contracts\Dtos\Dict\DictSearchPagedDto.cs`
- `references\src\Application\Contracts\Dtos\Dict\DictDto.cs`
- `references\src\Application\Contracts\Dtos\Dict\Validators\DictCreationDtoValidator.cs`
- `references\src\Application\Contracts\Dtos\Dict\Validators\DictUpdationDtoValidator.cs`
- `references\src\Application\Contracts\Dtos\SysConfig\Validators\SysConfigCreationDtoValidator.cs`
- `references\src\Api\RouteConsts.cs`
- `references\src\Api\Controllers\DictController.cs`
- `references\src\Api\Controllers\SysConfigController.cs`
- `references\src\Api\PermissionConsts.cs`

## Conventions to copy

This skill keeps mirrored reference snapshots under `references\src`, but generated output in the current workspace must go under `.\src\Repository`, `.\src\Application`, and `.\src\Api`. Do not write generated code into `references\src`. Generated code namespaces should still use the user-provided prefix plus `.Repository`, `.Application`, and `.Api`.

For speed, read these mirrored files once at the start of a generation request and then generate directly unless a concrete mismatch requires another lookup.

### Repository

- Entities live in `.\src\Repository\Entities`.
- Entity configurations live in `.\src\Repository\Entities\Config`.
- Entities usually inherit `EfFullAuditEntity`.
- Soft-delete tables also implement `ISoftDelete`.
- Tables with a real `rowversion` column also implement `IConcurrency` and keep a `RowVersion` property on the entity.
- String length constraints belong in `EntityConsts.cs` plus the corresponding `*Config.cs`.
- Table names are configured centrally in `EntityInfo.cs`.
- Shared registration files should be updated in the same generation pass as the per-entity files.

### Application

- DTOs live in `Contracts\Dtos\{Entity}` and validators live in `Contracts\Dtos\{Entity}\Validators`.
- Service interfaces live in `Contracts\Interfaces`.
- Implementations live in `Services`.
- AutoMapper registrations live in `MapperProfile.cs`.
- Use XML doc comments and English wording similar to the existing Dict and SysConfig files, except entity property `<summary>` text should come from the DDL column comment verbatim and otherwise fall back to the raw SQL column name.
- Use `ServiceResult`, `IdDto`, `PageModelDto<T>`, and `ExpressionCreator`.
- Use `TrimStringFields()` before business logic for write and search flows.
- Use `IdGenerater.GetNextId()` for inserts.

### API

- Controllers live in `.\src\Api\Controllers`.
- Controllers are thin and delegate to services.
- Use `CreatedResult(...)` and `Result(...)` from `AdncControllerBase`.
- Permission codes live in `PermissionConsts.cs`.
- Route prefixes come from `RouteConsts.AdminRoot`.

## Examples worth imitating

- Standard CRUD service: `DictService`, `SysConfigService`
- Standard CRUD controller: `DictController`, `SysConfigController`
- Standard validators: `DictCreationDtoValidator`, `SysConfigCreationDtoValidator`
- Standard table config: `DictConfig`, `UserConfig`

## Cases to treat carefully

- `Dict` and `DictData` are co-located in existing files, but new generated output should stay one class per file unless the user asks otherwise.
- `Organization` and `Menu` contain tree-specific behavior. Do not copy their hierarchy logic into unrelated tables.
- `User` includes `ISoftDelete`; only copy that behavior when the DDL actually contains a soft-delete column.
