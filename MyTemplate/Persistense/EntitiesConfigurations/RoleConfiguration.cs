namespace MyTemplate.Persistense.EntitiesConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {


        builder.HasData(
            new ApplicationRole
            {
                Id = Guid.Parse(DefaultRoles.Admin.Id),
                Name = DefaultRoles.Admin.Name,
                NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp,

            },
            new ApplicationRole
            {
                Id = Guid.Parse(DefaultRoles.Member.Id),
                Name = DefaultRoles.Member.Name,
                NormalizedName = DefaultRoles.Member.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Member.ConcurrencyStamp,

            }
        );
    }
}
