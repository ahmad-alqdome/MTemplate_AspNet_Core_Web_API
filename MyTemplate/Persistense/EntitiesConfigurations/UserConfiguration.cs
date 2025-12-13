namespace MyTemplate.Persistense.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {


        builder.HasData(new ApplicationUser
        {
            Id = Guid.Parse(DefaultUsers.Admin.Id),
            FullName = "App Admin",
            UserName = "Admin",
            NormalizedUserName = "Admin".ToUpper(),
            Email = DefaultUsers.Admin.Email,
            NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
            SecurityStamp = DefaultUsers.Admin.SecurityStamp,
            ConcurrencyStamp = DefaultUsers.Admin.ConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = DefaultUsers.Admin.PasswordHash,

        });
    }
}