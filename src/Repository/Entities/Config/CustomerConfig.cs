using Adnc.Skill.Test.Repository.Entities;

namespace Adnc.Skill.Test.Repository.Entities.Config;

public class CustomerConfig : AbstractEntityTypeConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).HasMaxLength(CustomerConsts.Name_MaxLength);
        builder.Property(x => x.Phone).HasMaxLength(CustomerConsts.Phone_MaxLength);
    }
}
