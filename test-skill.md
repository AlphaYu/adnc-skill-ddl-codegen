# Skill 测试用例清单

下面这 5 组就够了：它们能覆盖这个 skill 最关键的判断分支，而且都足够小，适合反复回归测试。统一搭配命名空间前缀 `Adnc.Demo.Admin` 使用。

## 1. 普通业务表

**目标**：验证标准 CRUD 生成

```sql
CREATE TABLE sys_customer (
  id bigint NOT NULL,
  name varchar(64) NOT NULL,
  phone varchar(32) NULL,
  status bit NOT NULL,
  createby bigint NULL,
  createtime datetime NULL,
  modifyby bigint NULL,
  modifytime datetime NULL,
  rowversion timestamp NULL,
  PRIMARY KEY (id)
);
```

## 2. 含软删除表

**目标**：验证 `ISoftDelete` / `IsDeleted` 处理

```sql
CREATE TABLE sys_notice (
  id bigint NOT NULL,
  title varchar(128) NOT NULL,
  content varchar(500) NULL,
  isdeleted bit NOT NULL,
  createby bigint NULL,
  createtime datetime NULL,
  modifyby bigint NULL,
  modifytime datetime NULL,
  rowversion timestamp NULL,
  PRIMARY KEY (id)
);
```

## 3. 含唯一键表

**目标**：验证 service 中唯一性检查

```sql
CREATE TABLE sys_tenant (
  id bigint NOT NULL,
  code varchar(32) NOT NULL,
  name varchar(128) NOT NULL,
  status bit NOT NULL,
  createby bigint NULL,
  createtime datetime NULL,
  modifyby bigint NULL,
  modifytime datetime NULL,
  rowversion timestamp NULL,
  PRIMARY KEY (id),
  UNIQUE (code)
);
```

## 4. 纯关系表

**目标**：验证不会默认直接生成全套 CRUD，而是先询问

```sql
CREATE TABLE role_user_relation (
  id bigint NOT NULL,
  role_id bigint NOT NULL,
  user_id bigint NOT NULL,
  createby bigint NULL,
  createtime datetime NULL,
  modifyby bigint NULL,
  modifytime datetime NULL,
  rowversion timestamp NULL,
  PRIMARY KEY (id)
);
```

## 5. 不完整 DDL

**目标**：验证缺少真实 schema 时会停止并索要完整 DDL

```sql
-- 故意只给片段
CREATE TABLE sys_order (
  id bigint NOT NULL,
  order_no varchar(64) NOT NULL
);
```

## 统一提示词模板

每个用例都用同一类提示词测试：

```text
根据下面的 DDL 生成 Adnc CRUD 代码。
namespace prefix: Adnc.Skill.Test
DDl文件:C:\personal\github\adnc-skill\adnc-skill-ddl-codegen\test-dbscript.sql
```

## 每组重点断言

1. **普通业务表**
   - 生成完整文件集：Entity、Config、4 个 DTO、2 个 Validator、Service、Controller
   - 更新 `EntityInfo.cs`、`MapperProfile.cs`、`PermissionConsts.cs`
   - `name/phone` 长度进入 config/validator
   - DTO 不包含审计字段

2. **软删除表**
   - Entity 实现 `ISoftDelete`
   - 保留 `IsDeleted`
   - `CreationDto` / `UpdationDto` 不暴露 `IsDeleted`

3. **唯一键表**
   - Service 在 create/update 中体现唯一性检查
   - 错误语义符合 `"... already exists"`

4. **关系表**
   - 先问是否需要完整 CRUD
   - 不应无条件生成 controller/service 全套

5. **不完整 DDL**
   - 明确停止
   - 要求补充真实完整 `CREATE TABLE`
   - 不根据表名或片段猜字段

## 最小冒烟测试顺序

1. `sys_customer`
2. `sys_notice`
3. `sys_tenant`
4. `role_user_relation`
5. `sys_order`

这样一套已经足够判断这个 skill 是否“会先读规则、能正确分类、不会猜、能生成完整面”。
