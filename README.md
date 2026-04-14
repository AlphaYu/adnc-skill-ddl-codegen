# adnc-skill-ddl-codegen

一个用于**编写、维护、测试 skill 文件**的仓库，目标是指导代理根据 SQL DDL 生成符合约定的 Adnc Admin CRUD 代码。

## 仓库定位

- 这里不是业务系统源码仓库
- 主要产物是 skill、模板、规则说明和参考快照
- 生成代码时，目标输出目录是 `.\src`
- `references\src` 仅作为参考样例，不是生成输出目录

## 关键文件

| 文件 | 作用 |
| --- | --- |
| `SKILL.md` | 主 skill 合同，定义触发条件、输入要求、工作流、输出范围和生成规则 |
| `assets\templates.md` | 生成代码时使用的模板骨架 |
| `references\ddl-mapping.md` | DDL 到 Entity / DTO / Service / Controller 的映射规则 |
| `references\project-conventions.md` | 需要优先读取的参考文件和项目约定 |
| `INSTRUCTIONS.md` | 本仓库维护目标与修改原则 |

## 当前生成规则要点

1. 普通业务表默认直接生成完整 CRUD。
2. 关系表如 `*_relation` 默认跳过完整 CRUD，除非用户明确要求。
3. 实体属性 `<summary>` 优先使用 DDL 列 `COMMENT`，为空时回退到原始列名。
4. 表中有 `rowversion` 时，实体保留 `RowVersion` 并实现 `IConcurrency`。
5. 表中有 `isdeleted` 时，实体保留 `IsDeleted` 并实现 `ISoftDelete`。
6. 生成成功后不执行 `git add`，只返回 `生成成功`。

## 使用方式

提供以下输入即可触发生成：

- DDL 文件或 `CREATE TABLE` 片段
- namespace prefix，例如 `Adnc.Demo.Admin`

推荐 DDL 同时包含：

- 列 `COMMENT`
- `UNIQUE` 键或唯一索引
- `isdeleted`
- `rowversion`

## 维护建议

- 优先更新 `SKILL.md`、`assets\templates.md` 和 `references\*.md`
- 不要把 `references\src` 直接当成生成输出目录
- 如果生成风格需要调整，优先改规则和模板，不要直接用生成结果覆盖参考快照
