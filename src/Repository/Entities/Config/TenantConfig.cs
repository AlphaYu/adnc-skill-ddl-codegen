using Adnc.Skill.Test.Repository.Entities;

namespace Adnc.Skill.Test.Repository.Entities.Config;

public class TenantConfig : AbstractEntityTypeConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Code).HasMaxLength(TenantConsts.Code_MaxLength);
        builder.Property(x => x.Name).HasMaxLength(TenantConsts.Name_MaxLength);
    }
}
