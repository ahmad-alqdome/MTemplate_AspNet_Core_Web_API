namespace MyTemplate.Persistense.EntitiesConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        //Default Data
        builder.HasData(
            new IdentityUserRole<Guid>
            {
                UserId = Guid.Parse(DefaultUsers.Admin.Id),
                RoleId = Guid.Parse(DefaultRoles.Admin.Id),
            }

        );
    }
}
