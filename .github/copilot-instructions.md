# Copilot instructions

## Repository purpose

This repository is a **skill package**, not the Adnc business application itself. Most work here is about improving the agent guidance that generates Adnc Admin CRUD code from SQL DDL.

- `SKILL.md` is the main contract: trigger conditions, required inputs, workflow, output scope, and repository-specific generation rules.
- `INSTRUCTIONS.md` explains the repo's purpose: maintain the skill, templates, and reference materials rather than adding business code here.
- `assets\templates.md` contains the skeletons the skill should follow when generating code.
- `references\` contains mirrored source snapshots and rule docs from the target Adnc repository. Treat these as **reference inputs**, not output locations. DDL must be provided or selected manually by the user.

## Commands

This repo does **not** define a local build, test, or lint workflow.

The only concrete validation command documented here is for the **target Adnc repository** after generated C# files change:

```powershell
dotnet build src\Adnc.Demo.sln
```

No repo-local single-test command is documented.

## High-level architecture

The skill works in three layers that mirror the target Adnc Admin module:

| Layer | Target output | Key shared updates |
| --- | --- | --- |
| Repository | `src\Repository\Entities`, `src\Repository\Entities\Config` | `EntityInfo.cs`, `EntityConsts.cs` |
| Application | `src\Application\Contracts\Dtos`, `Contracts\Interfaces`, `Services` | `MapperProfile.cs` |
| API | `src\Api\Controllers` | `PermissionConsts.cs` |

The repository is organized around supporting that workflow:

1. `SKILL.md` defines when the skill should run and what files it must create or update.
2. `references\project-conventions.md` points to the mirrored files that should be read before generating anything.
3. `references\ddl-mapping.md` maps SQL tables and columns to entity, DTO, validator, service, and controller decisions.
4. `assets\templates.md` provides the file skeletons for generated output.
5. `references\src` provides concrete examples from the target repo so the skill can imitate real patterns without depending on a live checkout.

## Key conventions

- Read `references\.editorconfig`, `references\ddl-mapping.md`, and `references\project-conventions.md` before changing the skill or templates.
- Keep `SKILL.md`, `INSTRUCTIONS.md`, `assets\templates.md`, and relevant `references\*.md` files aligned when a rule changes.
- Use the mirrored files under `references\src` to learn conventions; generated output must be written under `.\src\Repository`, `.\src\Application`, and `.\src\Api` in the current workspace, not into `references\src`.
- After generation succeeds, do not stage generated files with git; reply with exactly `生成成功`.
- DDL must come from a user-provided or user-selected source. If the real DDL is incomplete, stop and ask for the actual `CREATE TABLE` definition.
- For the fastest path, prefer DDL that includes column comments, unique keys or indexes, and explicit `isdeleted` or `rowversion` columns when needed.
- Require the user to provide the namespace prefix explicitly. Generated namespaces append only `.Repository`, `.Application`, and `.Api`.
- Distinguish normal business tables from pure relation tables. For join tables such as `*_relation`, skip full CRUD by default unless the user explicitly asks for it.
- Follow the documented naming rules: snake_case SQL names become PascalCase C# names, but preserve established repo-specific names like `Dict`, `DictData`, and `SysConfig` when they are explicitly documented.
- Match the mirrored C# style from `references\.editorconfig`: file-scoped namespaces, `using` directives outside the namespace, sorted usings, and broad `var` usage.
- Keep new generated output to one class per file even if some reference files co-locate related types.
- Generate English XML doc comments to match the mirrored source files, except entity property `<summary>` text in `.\src\Repository\Entities` must come from the DDL column comment verbatim and otherwise fall back to the raw SQL column name.
- Repository entities usually inherit `EfFullAuditEntity`; keep `IsDeleted` and implement `ISoftDelete` when the DDL contains that column, and keep `RowVersion` plus implement `IConcurrency` when the DDL contains a `rowversion` column.
- DTO generation follows the documented inheritance pattern: `CreationDto : InputDto`, `UpdationDto : CreationDto`, `SearchPagedDto : SearchPagedDto`, `Dto : CreationDto + Id`.
- Services should mirror the reference CRUD flow: use `IEfRepository<T>`, `IdGenerater.GetNextId()`, `input.TrimStringFields()`, `ExpressionCreator.New<T>()`, and `Problem(...)` for not-found or uniqueness failures.
- Controllers stay thin, inherit `AdncControllerBase`, use `CreatedResult(...)` or `Result(...)`, route under `RouteConsts.AdminRoot`, and authorize through nested `PermissionConsts` entries.
- Shared files are part of the contract, not optional follow-up work: update `EntityInfo.cs`, `MapperProfile.cs`, and `PermissionConsts.cs` for every normal business entity, plus `EntityConsts.cs` when new reusable length constants are needed.
- Read the required references once per request, then generate directly; do not repeat the same reference reads unless a concrete conflict appears in the output.
