# adnc-skill-ddl-codegen

这个仓库用于维护一个 **skill 包**，指导代理根据 SQL DDL 生成符合约定的 Adnc Admin CRUD 代码。它的重点是编写、迭代和验证 skill 本身，而不是开发业务系统。

## 仓库定位

- 在 `SKILL.md` 中维护生成合同
- 保持模板和规则文档与目标 Adnc 仓库约定一致
- 使用 `references\src` 下的镜像源码快照学习生成模式
- 真正的生成输出目录是 `.\src`，不是 `references\src`

## 仓库结构

| 路径 | 作用 |
| --- | --- |
| `SKILL.md` | 主 skill 合同：触发条件、必需输入、工作流、输出范围和生成规则 |
| `INSTRUCTIONS.md` | 仓库目标和维护原则 |
| `assets\templates.md` | Entity、DTO、Validator、Service、Controller 以及共享更新的模板骨架 |
| `references\ddl-mapping.md` | SQL DDL 到 Adnc Repository / Application / Api 代码的映射规则 |
| `references\project-conventions.md` | 必读参考文件和项目约定 |
| `references\src` | 仅用于模仿风格的镜像源码快照 |

## skill 预期生成内容

对于普通业务表，skill 应生成或更新：

- `.\src\Repository\Entities\{Entity}.cs`
- `.\src\Repository\Entities\Config\{Entity}Config.cs`
- `.\src\Repository\EntityInfo.cs`
- `.\src\Repository\EntityConsts.cs`（当需要复用长度常量时）
- `.\src\Application\Contracts\Dtos\{Entity}\*`
- `.\src\Application\Contracts\Interfaces\I{Entity}Service.cs`
- `.\src\Application\Services\{Entity}Service.cs`
- `.\src\Application\MapperProfile.cs`
- `.\src\Api\Controllers\{Entity}Controller.cs`
- `.\src\Api\PermissionConsts.cs`

像 `*_relation` 这样的纯关系表，默认跳过完整 CRUD，只有在用户明确要求时才生成完整 CRUD。

## 必需输入

只有在用户提供以下内容时，skill 才应执行：

1. 目标表的 DDL 文件或 `CREATE TABLE` 语句
2. 命名空间前缀，例如 `Adnc.Demo.Admin`

为了获得更快且更稳定的生成结果，DDL 最好同时包含：

- 列 `COMMENT`
- 显式 `UNIQUE` 键或唯一索引
- 明确的 `isdeleted` 和 `rowversion` 列（如果需要这些行为）

如果 DDL 信息不完整，skill 必须停止并索取真实表定义，不能根据残缺 SQL 或 seed data 猜测结构。

## 如何使用

使用这个 skill 时，最好在同一条提示词里同时提供明确任务、DDL 和命名空间前缀。

推荐流程：

1. 提供一个或多个 `CREATE TABLE` 语句。
2. 明确写出 namespace prefix。
3. 如果表类型不明显，说明它是普通业务表还是关系表。
4. 只有在你想缩小默认完整 CRUD 范围时，再补充额外范围限制。

提示词示例：

```text
根据下面的 DDL 生成 Adnc CRUD 代码。
Namespace prefix: Adnc.Demo.Admin

CREATE TABLE sys_customer (
    id bigint NOT NULL,
    customer_code varchar(32) NOT NULL COMMENT '客户编码',
    customer_name varchar(128) NOT NULL COMMENT '客户名称',
    isdeleted bit NOT NULL DEFAULT 0 COMMENT '删除标记',
    rowversion rowversion NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uk_customer_code (customer_code)
);
```

## 如何写提示词

好的提示词不一定很长，但一定要完整。通常应包含：

- 动作：generate、update、regenerate 等
- 目标表
- DDL 本身，或由用户明确指定的 DDL 来源
- namespace prefix
- 非默认要求，比如关系表也要生成完整 CRUD，或者只生成部分层

推荐模板：

```text
Generate Adnc CRUD code for these tables.
Namespace prefix: <Your.Namespace.Prefix>
Special requirements: <only if needed>

<CREATE TABLE ...>
```

写提示词时建议：

- 尽量直接提供真实 `CREATE TABLE` 语句，不要只用自然语言描述字段。
- 如果希望实体属性注释更准确，DDL 里要带列 `COMMENT`。
- 如果希望 service 自动生成唯一性检查，DDL 里要带唯一键或唯一索引。
- 如果目标是纯关系表且你仍然想生成完整 CRUD，要明确写出 `generate relation-table CRUD`。
- 不要让 skill 从 insert 脚本、截图或残缺 SQL 中反推完整表结构。

示例：

- **普通业务表完整 CRUD**：`Generate Adnc Admin CRUD code for this table. Namespace prefix: Adnc.Demo.Admin`
- **强制为关系表生成 CRUD**：`Generate relation-table CRUD for this DDL. Namespace prefix: Adnc.Demo.Admin`
- **限制生成范围**：`Generate only repository and application layers for this DDL. Namespace prefix: Adnc.Demo.Admin`

## 核心规则

1. 在调整生成行为前，先读取 `references\.editorconfig`、`references\ddl-mapping.md` 和 `references\project-conventions.md`。
2. 规则变化时，要同步保持 `SKILL.md`、`INSTRUCTIONS.md`、`assets\templates.md` 和相关 `references\*.md` 一致。
3. 生成代码只能写入 `.\src\Repository`、`.\src\Application` 和 `.\src\Api`，不能写入 `references\src`。
4. 实体属性的 `<summary>` 必须优先使用 DDL 列注释原文；没有注释时回退到原始 SQL 列名。
5. 当 DDL 包含 `isdeleted` 时保留 `IsDeleted` 并实现 `ISoftDelete`；当包含 `rowversion` 时保留 `RowVersion` 并实现 `IConcurrency`。
6. `EntityInfo.cs`、`MapperProfile.cs`、`PermissionConsts.cs` 等共享文件必须和实体文件在同一轮一起更新。
7. 生成成功后不要执行 `git add` 或暂存生成文件，最终回复必须严格为 `生成成功`。

## 维护建议

- 优先改规则和模板，不要把参考快照当作产出目录
- 将 `references\src` 视为只读参考材料
- 尽量把高频判断固化为明确默认规则，减少重复确认
- 优先保证生成结果可预测、命名清晰、共享文件更新完整

## LICENSE

本仓库采用 MIT License。

完整许可文本请查看根目录下的 `LICENSE` 文件。
