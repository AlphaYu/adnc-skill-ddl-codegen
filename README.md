# <div align="center"><img src="https://aspdotnetcore.net/wp-content/uploads/2023/04/adnc-github.png" alt="Adnc.Codegen" style="zoom:50%;" /></div>

<div align='center'>
<a href="https://github.com/AlphaYu/Adnc/blob/master/LICENSE">
<img alt="GitHub license" src="https://img.shields.io/github/license/AlphaYu/Adnc"/>
</a>
<a href="https://github.com/AlphaYu/Adnc/stargazers">
<img alt="GitHub stars" src="https://img.shields.io/github/stars/AlphaYu/Adnc"/>
</a>
<a href="https://github.com/AlphaYu/Adnc/network">
<img alt="GitHub forks" src="https://img.shields.io/github/forks/AlphaYu/Adnc"/>
</a>
<a href="">
<img alt="Visitors" src="https://komarev.com/ghpvc/?username=alphayu&color=red&label=Visitors"/>
</a>
</div>

[中文](./README_ZH.md)  [English](./README.md)

This repository maintains a **skill package** that guides an agent to generate Adnc CRUD code from SQL DDL. It is for authoring, refining, and validating the skill itself rather than building the business application.

## Purpose

- Maintain the generation contract in `SKILL.md`
- Keep templates and rule documents aligned with the target Adnc repository
- Use mirrored source snapshots under `references\src` as examples for generation behavior
- Generate real output under `.\src`, never under `references\src`

## Repository layout

| Path | Role |
| --- | --- |
| `SKILL.md` | Main skill contract: triggers, required inputs, workflow, output scope, and generation rules |
| `INSTRUCTIONS.md` | Repository purpose and maintenance guidance |
| `assets\templates.md` | Skeletons for generated entities, DTOs, validators, services, controllers, and shared updates |
| `references\ddl-mapping.md` | Mapping rules from SQL DDL to Adnc repository, application, and API code |
| `references\project-conventions.md` | Required reference files and repo-specific conventions |
| `references\src` | Mirrored source snapshots for pattern matching only |

## What the skill generates

For a normal business table, the skill is expected to generate or update:

- `.\src\Repository\Entities\{Entity}.cs`
- `.\src\Repository\Entities\Config\{Entity}Config.cs`
- `.\src\Repository\EntityInfo.cs`
- `.\src\Repository\EntityConsts.cs` when reusable length constants are needed
- `.\src\Application\Contracts\Dtos\{Entity}\*`
- `.\src\Application\Contracts\Interfaces\I{Entity}Service.cs`
- `.\src\Application\Services\{Entity}Service.cs`
- `.\src\Application\MapperProfile.cs`
- `.\src\Api\Controllers\{Entity}Controller.cs`
- `.\src\Api\PermissionConsts.cs`

Pure relation tables such as `*_relation` should skip full CRUD by default unless the user explicitly asks for it.

## Required inputs

The skill should only run when the user provides:

1. A DDL file or `CREATE TABLE` statements for the target tables
2. The namespace prefix, for example `Adnc.Demo.Admin`

For the fastest and most reliable path, the DDL should also include:

- column `COMMENT` values
- explicit `UNIQUE` keys or unique indexes
- explicit `isdeleted` and `rowversion` columns when those behaviors are required

If the DDL is incomplete, the skill must stop and ask for the real table definition instead of guessing from partial SQL or seed data.

## How to use

Use the skill by giving the agent a clear task plus the required DDL and namespace prefix in the same prompt.

Recommended flow:

1. Provide one or more `CREATE TABLE` statements.
2. State the namespace prefix explicitly.
3. Say whether the target is a normal business table or a relation table if that is not obvious.
4. Add any scope limits only when you want to narrow the default full-CRUD output.

Example prompt:

```text
Generate Adnc CRUD code from the following DDL.
Namespace prefix: Adnc.Demo.Admin

CREATE TABLE sys_customer (
    id bigint NOT NULL,
    customer_code varchar(32) NOT NULL COMMENT 'Customer code',
    customer_name varchar(128) NOT NULL COMMENT 'Customer name',
    isdeleted bit NOT NULL DEFAULT 0 COMMENT 'Deletion flag',
    rowversion rowversion NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uk_customer_code (customer_code)
);
```

## How to write prompts

Good prompts are short but complete. A useful prompt usually contains:

- the action: generate, update, or regenerate
- the target tables
- the DDL itself or a clear DDL source selected by the user
- the namespace prefix
- any non-default requirement, such as relation-table CRUD or limited output scope

Recommended prompt pattern:

```text
Generate Adnc CRUD code for these tables.
Namespace prefix: <Your.Namespace.Prefix>
Special requirements: <only if needed>

<CREATE TABLE ...>
```

Prompt writing tips:

- Include the real `CREATE TABLE` statement instead of describing columns in prose.
- Include column comments when you want accurate entity property XML docs.
- Include unique keys or indexes when uniqueness checks should be generated in services.
- Explicitly say `generate relation-table CRUD` when the table is a pure join table and you do want full CRUD.
- Do not ask the skill to infer schema from insert scripts, screenshots, or partial SQL.

Examples:

- **Full CRUD for a normal table**: `Generate Adnc Admin CRUD code for this table. Namespace prefix: Adnc.Demo.Admin`
- **Force CRUD for a relation table**: `Generate relation-table CRUD for this DDL. Namespace prefix: Adnc.Demo.Admin`
- **Restrict scope**: `Generate only repository and application layers for this DDL. Namespace prefix: Adnc.Demo.Admin`

## Core rules

1. Read `references\.editorconfig`, `references\ddl-mapping.md`, and `references\project-conventions.md` before changing generation behavior.
2. Keep `SKILL.md`, `INSTRUCTIONS.md`, `assets\templates.md`, and relevant `references\*.md` files aligned when rules change.
3. Generate code into `.\src\Repository`, `.\src\Application`, and `.\src\Api`; never write generated output into `references\src`.
4. Entity property `<summary>` text must use the DDL column comment verbatim, or fall back to the raw SQL column name when no comment is available.
5. Preserve `IsDeleted` with `ISoftDelete` when the DDL contains `isdeleted`, and preserve `RowVersion` with `IConcurrency` when the DDL contains `rowversion`.
6. Update shared files such as `EntityInfo.cs`, `MapperProfile.cs`, and `PermissionConsts.cs` in the same pass as per-entity generation.
7. After successful generation, do not stage files with git; the completion message must be exactly `生成成功`.

## Maintenance guidance

- Prefer changing rules and templates over editing generated examples
- Treat `references\src` as read-only reference material
- Keep defaults explicit so the skill can take the fast path without repeated confirmation
- Optimize for deterministic generation, clear naming, and complete shared-file updates

## License

This project is licensed under the **MIT License**. See [LICENSE](./LICENSE) for details.
