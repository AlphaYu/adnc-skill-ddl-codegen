---
name: adnc-skill-ddl-codegen
description: Generates Adnc Repository, Application, and Api CRUD code from SQL DDL or CREATE TABLE statements for this repository. Use when asked to add entities, entity configs, EntityInfo mappings, DTOs, validators, service interfaces, services, controllers, MapperProfile mappings, or PermissionConsts entries based on one or more tables.
compatibility: Designed for GitHub Copilot CLI or similar coding agents with read/write access to this repository and a user-provided DDL file or snippet.
metadata:
  repo: adnc-monolithic
  module: adnc
  layer-scope: repository-application-api
---

# Adnc DDL code generation

Use this skill when the task is to generate adnc module code from one or more table DDL definitions.

Typical triggers:

- "根据 DDL 生成 Adnc 代码"
- "为新表生成 entity / dto / service / controller"
- "Add CRUD code for a new table"
- "Update EntityInfo, MapperProfile, and PermissionConsts for new tables"

## Required inputs

1. A target table list or a DDL file/snippet containing `CREATE TABLE` definitions.
2. The namespace prefix to use for generated code, entered manually by the user, for example `Adnc.Demo.Admin`.
3. The DDL source must be selected or provided manually by the user. Do not fall back to repository files as an implicit schema source.
4. If the available SQL does **not** contain enough schema information for the target table, stop and ask for the real DDL instead of guessing from partial SQL or seed data.

Preferred DDL content for the fastest path:

- column `COMMENT` values for entity property `<summary>` generation
- explicit `UNIQUE` keys or unique indexes for service uniqueness checks
- explicit `isdeleted` and `rowversion` columns when those behaviors are required

## Fast-path defaults

Use these defaults to minimize generation time:

1. Read the required reference files once per request, then generate directly. Do not re-read the same reference files unless the current output reveals a concrete conflict.
2. For normal business tables, generate the full CRUD file set immediately without asking for extra confirmation.
3. For pure relation tables such as `*_relation`, skip full CRUD by default. Only generate the full CRUD surface when the user explicitly asks for relation-table CRUD.
4. Update `EntityInfo.cs`, `MapperProfile.cs`, `PermissionConsts.cs`, and `EntityConsts.cs` in the same pass as the per-entity files. Do not treat shared file updates as optional follow-up work.
5. Ask follow-up questions only when one of these blockers is true:
   - the DDL is incomplete
   - the namespace prefix is missing
   - the user explicitly asks for relation-table CRUD
   - a real naming conflict or ambiguity cannot be resolved from the documented rules
6. Keep the final response minimal: after successful generation, reply with exactly `生成成功`.

## Always read these files first

Read `references\.editorconfig` and the mirrored implementation files listed in `references/project-conventions.md`.

Read `references/ddl-mapping.md` before generating any code from SQL.

Read `assets/templates.md` when writing the actual files.

The skill ships with mirrored project snapshots under `references\src`. Use those copies for conventions and examples instead of depending on the live repository layout.

## Workflow

1. Identify the target tables and decide whether each is a normal business table or a pure relation table.
2. Read the current repository patterns once before editing anything.
3. Convert each target table into an entity name and output file set under `.\src`.
4. Generate repository files first:
   - `.\src\Repository\Entities\{Entity}.cs`
   - `.\src\Repository\Entities\Config\{Entity}Config.cs`
   - `.\src\Repository\EntityInfo.cs`
   - `.\src\Repository\EntityConsts.cs` when new max-length constants are needed
5. Generate application files:
   - `.\src\Application\Contracts\Interfaces\I{Entity}Service.cs`
   - `.\src\Application\Services\{Entity}Service.cs`
   - `.\src\Application\Contracts\Dtos\{Entity}\{Entity}CreationDto.cs`
   - `.\src\Application\Contracts\Dtos\{Entity}\{Entity}UpdationDto.cs`
   - `.\src\Application\Contracts\Dtos\{Entity}\{Entity}SearchPagedDto.cs`
   - `.\src\Application\Contracts\Dtos\{Entity}\{Entity}Dto.cs`
   - `.\src\Application\Contracts\Dtos\{Entity}\Validators\{Entity}CreationDtoValidator.cs`
   - `.\src\Application\Contracts\Dtos\{Entity}\Validators\{Entity}UpdationDtoValidator.cs`
   - `.\src\Application\MapperProfile.cs`
6. Generate API files:
   - `.\src\Api\Controllers\{Entity}Controller.cs`
   - `.\src\Api\PermissionConsts.cs`
7. Validate naming, namespaces, XML docs, DDL comment usage, and analyzer-sensitive formatting.
8. After creating the code files, do **not** run `git add` or stage the generated files. Finish by replying with exactly `生成成功`.

The file paths above are rooted at `.\src` in the current workspace. They are **not** namespace templates, and generated code must not be written to `references\src`.

## Output contract

For each normal business table, generate all of the following unless the user explicitly narrows scope:

- one entity file
- one entity configuration file
- one service interface
- one service implementation
- four DTO files
- two validator files
- one controller
- shared file updates for `EntityInfo.cs`, `MapperProfile.cs`, and `PermissionConsts.cs`
- write all generated and updated code files under `.\src`
- do not add generated files to git; the completion message should be exactly `生成成功`

## Repository-specific rules

### Naming and structure

- Follow file-scoped namespaces.
- Keep `using` directives outside the namespace.
- Never assume the namespace prefix. Require the user to provide it explicitly and apply that prefix consistently to Repository, Application, and Api namespaces.
- Namespace suffixes must be `{{NamespacePrefix}}.Repository`, `{{NamespacePrefix}}.Application`, and `{{NamespacePrefix}}.Api`. Do not generate `{{NamespacePrefix}}.Admin.Repository`, `{{NamespacePrefix}}.Admin.Application`, or `{{NamespacePrefix}}.Admin.Api`.
- Prefer one generated class per file, even if some existing files co-locate closely related types.
- Convert snake_case table and column names to PascalCase property and type names.
- Repository local variables should use camelCase names such as `customerRepo` and `transactionLogRepo`.

### Entity rules

- Default base class: `EfFullAuditEntity`.
- If the table includes `isdeleted`, implement `ISoftDelete` and keep the `IsDeleted` property on the entity.
- If the table includes `rowversion`, implement `IConcurrency` on the entity and keep the `RowVersion` property on the entity.
- Do **not** regenerate audit/base properties already covered by the framework unless they must remain explicit in the entity:
  - `Id`
  - `CreateBy`
  - `CreateTime`
  - `ModifyBy`
  - `ModifyTime`
- Keep `RowVersion` out of create and update DTOs even when the entity keeps it for concurrency.
- String properties should use `string` plus `= string.Empty;`.
- Nullable value types should stay nullable, for example `DateTime?`, `int?`, `decimal?`.
- For entity properties under `.\src\Repository\Entities`, the `<summary>` text must come from the DDL column `COMMENT` value.
- If a DDL column comment is empty or missing, use the original SQL column name as the property `<summary>` text.
- Do not replace entity property `<summary>` text with guessed English wording when a DDL column comment is available.

### DTO rules

- `CreationDto` inherits `InputDto`.
- `UpdationDto` inherits `{Entity}CreationDto`.
- `{Entity}SearchPagedDto` inherits `SearchPagedDto`.
- `{Entity}Dto` inherits `{Entity}CreationDto` and adds `long Id`.
- Exclude these fields from `CreationDto`, `UpdationDto`, and `{Entity}Dto` inheritance source generation:
  - `Id`
  - `CreateBy`
  - `CreateTime`
  - `ModifyBy`
  - `ModifyTime`
  - `IsDeleted`
  - `RowVersion`
- Add extra search properties to `{Entity}SearchPagedDto` only when the DDL or user requirement makes the filter obvious. Otherwise keep it empty.

### Validator rules

- Put validators under `Contracts\Dtos\{Entity}\Validators`.
- Add length rules from DDL string lengths or constants added to `EntityConsts.cs`.
- Add `NotEmpty()` only for required business fields.
- `UpdationDtoValidator` should `Include(new {Entity}CreationDtoValidator());`.

### Service rules

- Interface must inherit `IAppService`.
- Service class must inherit `AbstractAppService` and implement `I{Entity}Service`.
- Use `IEfRepository<{Entity}>`.
- Use `IdGenerater.GetNextId()` for inserts.
- Use `input.TrimStringFields();` in create, update, and paged-search methods when string fields are present.
- Use `Problem(HttpStatusCode.NotFound, "... does not exist")` for missing entities.
- Use `Problem(HttpStatusCode.BadRequest, "... already exists")` for unique-conflict checks inferred from unique keys or indexes.
- Use `Mapper.Map<{Entity}>(input, IdGenerater.GetNextId())` for create and `Mapper.Map(input, entity)` for update.
- Use `ExpressionCreator.New<{Entity}>().AndIf(...)` for paged query filters.
- Return `new PageModelDto<{Entity}Dto>(input)` when the total count is zero.
- Keep controllers thin and keep business logic in services.
- Do not add caching attributes or extra service methods unless the feature matches an existing cache-backed pattern in this repo.

### Controller rules

- Controller must inherit `AdncControllerBase`.
- Route pattern should be REST-like and consistent with existing controllers.
- Add actions for `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `GetPagedAsync`, and `GetAsync`.
- Use `CreatedResult(...)` for create and `Result(...)` for update and delete.
- Use `[AdncAuthorize(PermissionConsts.{Entity}.Create)]`, etc.
- The get-by-id action should authorize both `Get` and `Update`, matching current repo patterns.

### Shared file rules

- `EntityInfo.cs`: add the exact table mapping with `ToTable("actual_table_name")`.
- `MapperProfile.cs`: add `{Entity}CreationDto -> {Entity}`, `{Entity}UpdationDto -> {Entity}`, and `{Entity} -> {Entity}Dto`.
- `PermissionConsts.cs`: add a nested static class with `Create`, `Update`, `Delete`, `Search`, and `Get`.
- `EntityConsts.cs`: add a new const class only when the table introduces new bounded string fields that need max-length reuse.
- When shared files already exist, update them directly in the same generation pass instead of pausing for extra confirmation.

## Gotchas

- Do not infer a full schema from partial SQL, seed data, or table names when the target table DDL is missing.
- Do not default the namespace prefix to `Adnc.Demo.Admin`. If the user did not supply the prefix, stop and ask for it.
- This repo already has relation entities such as `RoleMenuRelation` and `RoleUserRelation` without generated CRUD controllers and services. For pure join tables, skip full CRUD by default unless the user explicitly requests it.
- Existing code sometimes uses the shared `SearchPagedDto` directly, but this skill must still create `{Entity}SearchPagedDto` because the requested output explicitly requires it.
- Keep XML comments in English to match the surrounding codebase, except entity property `<summary>` text in `.\src\Repository\Entities`, which must preserve the DDL column comment verbatim and otherwise fall back to the raw column name.
- Follow `references\.editorconfig` exactly: file-scoped namespaces, braces, `var`, and sorted usings.
- Do not run `git add`, do not stage generated files, and do not summarize file lists in the final reply; return exactly `生成成功` after generation succeeds.

## Validation checklist

Before finishing:

1. Confirm every target table produced the expected file set.
2. Confirm every new namespace matches its folder.
3. Confirm `EntityInfo.cs`, `MapperProfile.cs`, and `PermissionConsts.cs` were updated.
4. Confirm excluded audit fields were not added to the create/update DTOs.
5. Confirm string max lengths are enforced in config and validators.
6. Confirm entities with a `rowversion` column implement `IConcurrency` and keep a `RowVersion` property.
7. Confirm every entity property `<summary>` comes from the DDL column comment, or the raw column name when the comment is empty.
8. Run `dotnet build src\Adnc.Demo.sln` when code files changed.
