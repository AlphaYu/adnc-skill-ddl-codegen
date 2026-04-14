-- 验证标准 CRUD 生成
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
-- 含软删除表
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
--含唯一键表
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
-- 纯关系表,验证不会默认直接生成全套 CRUD，而是先询问
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