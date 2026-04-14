using System.Collections.Generic;
using System.Reflection;

using Adnc.Skill.Test.Repository.Entities;

using Microsoft.EntityFrameworkCore;

namespace Adnc.Skill.Test.Repository;

public class EntityInfo : AbstractEntityInfo
{
    protected override List<Assembly> GetEntityAssemblies() => [GetType().Assembly];

    protected override void SetTableName(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().ToTable("sys_customer");
        modelBuilder.Entity<Notice>().ToTable("sys_notice");
        modelBuilder.Entity<Tenant>().ToTable("sys_tenant");
    }
}
