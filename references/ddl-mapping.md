# DDL to Admin code mapping

## Table selection

- Generate the full CRUD surface only for normal business tables.
- For pure relation tables such as `{left}_{right}_relation`, skip full CRUD by default.
- Only generate services and controllers for relation tables when the user explicitly asks for relation-table CRUD.

## Fast-path input hints

To keep generation fast and deterministic, prefer DDL that includes:

- column `COMMENT` values
- explicit `UNIQUE` keys or indexes
- explicit `isdeleted` and `rowversion` columns when those behaviors are intended

## Name conversion

- Table name: snake_case to PascalCase.
- Column name: snake_case to PascalCase property name.
- Keep repository abbreviations already established in this repo when they clearly exist:
  - `sys_dictionary` -> `Dict`
  - `sys_dictionary_data` -> `DictData`
  - `sys_config` -> `SysConfig`
- Otherwise prefer readable PascalCase names, for example:
  - `sys_customer` -> `Customer`
  - `transaction_log` -> `TransactionLog`

## Base and audit columns

Treat these as framework/base concerns unless the entity must expose them explicitly:

- `id`
- `createby`
- `createtime`
- `modifyby`
- `modifytime`
- `rowversion`

If the table contains `isdeleted`, add `ISoftDelete` to the entity and keep:

```csharp
public bool IsDeleted { get; set; }
```

If the table contains `rowversion`, add `IConcurrency` to the entity and keep:

```csharp
public byte[] RowVersion { get; set; } = Array.Empty<byte>();
```

Do not include audit and soft-delete columns in the create/update DTO payloads.

## Type mapping

Use the repo's normal C# types:

| SQL shape | C# type |
| --- | --- |
| `bigint` | `long` |
| `int` | `int` |
| `smallint` | `short` |
| `tinyint(1)` / `bit` | `bool` |
| `decimal(p,s)` | `decimal` |
| nullable numeric | nullable numeric |
| `datetime`, `timestamp` | `DateTime` |
| nullable datetime | `DateTime?` |
| `varchar`, `char`, `text` | `string` initialized to `string.Empty` |

Use nullable value types only when the SQL column is nullable.

## Entity generation

For each target table:

1. Generate an entity with business columns only.
2. Add XML comments for public members.
3. For entity properties, use the DDL column `COMMENT` text as the `<summary>` content.
4. If the DDL column comment is empty or missing, use the original SQL column name as the property `<summary>` content.
5. Add string max-length config in `Entities\Config\{Entity}Config.cs`.
6. Add any reusable max lengths to `EntityConsts.cs`.

Do not invent EF navigation properties, concurrency handling, or index builders unless the existing code nearby already uses them or the user explicitly asks for them.

The exception is a real `rowversion` column from the DDL: in that case, keep `RowVersion` on the entity and implement `IConcurrency`.

Do not rewrite entity property `<summary>` text into English when the DDL already provides a column comment; preserve the source comment exactly.


## DTO generation

Generate these files under `Contracts\Dtos\{Entity}`:

- `{Entity}CreationDto.cs`
- `{Entity}UpdationDto.cs`
- `{Entity}SearchPagedDto.cs`
- `{Entity}Dto.cs`

DTO field guidance:

- Include all business columns that can be written by the caller.
- Exclude audit columns, `Id`, `IsDeleted`, and `RowVersion`.
- Keep `{Entity}Dto` inheritance simple: inherit from `{Entity}CreationDto` and add `Id`.
- Keep `{Entity}UpdationDto` empty unless the user asks for extra update-only fields.
- Keep `{Entity}SearchPagedDto` empty unless obvious filters are needed. Good candidates:
  - foreign key filters like `CustomerId`
  - code filters like `TypeCode`
  - status filters like `bool? Status`

## Validator generation

- Use `NotEmpty()` for required string and key fields.
- Use `.Length(min, max)` or `.MaximumLength(max)` based on the DDL and current repo style.
- Prefer length constants from `EntityConsts.cs`.
- Do not add uniqueness checks in validators; keep uniqueness in services.

## Service generation

Create standard CRUD members:

- `CreateAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAsync`
- `GetPagedAsync`

Service behavior rules:

- Trim string fields before validation-sensitive logic.
- Check uniqueness in `CreateAsync` and `UpdateAsync` only when the DDL declares a unique key or unique index, or when the user explicitly says a field must be unique.
- Use friendly singular error text like `"This customer does not exist"` or `"This transaction log already exists"` when no better domain phrasing is obvious.
- Order paged results by descending `Id` unless the table clearly needs another default order.

## API generation

Default action set:

- `POST /api/admin/{resource}`
- `PUT /api/admin/{resource}/{id}`
- `DELETE /api/admin/{resource}/{ids}`
- `GET /api/admin/{resource}/page`
- `GET /api/admin/{resource}/{id}`

Resource naming guidance:

- Prefer kebab-case route segments.
- Prefer plural route names when they read naturally, for example `customers` or `transaction-logs`.
- Keep permission constants stable and singular in meaning, for example `customer-create`, `transactionlog-search`.

## Shared updates

After generating per-table files, update:

1. `EntityInfo.cs`
2. `MapperProfile.cs`
3. `PermissionConsts.cs`
4. `EntityConsts.cs` when new string max-length constants are required

Update these shared files in the same pass as file generation; do not defer them to a second confirmation step.
