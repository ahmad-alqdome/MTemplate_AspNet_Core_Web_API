using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace MyTemplate.Persistense;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
    {
    }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //builder.Entity<RolePermission>(entity =>
        //{
        //    entity.HasKey(x => new { x.RoleId, x.PermissionId });

        //    entity.HasOne(x => x.Role)
        //          .WithMany()
        //          .HasForeignKey(x => x.RoleId)
        //          .OnDelete(DeleteBehavior.Cascade);

        //    entity.HasOne(x => x.Permission)
        //          .WithMany()
        //          .HasForeignKey(x => x.PermissionId)
        //          .OnDelete(DeleteBehavior.Cascade);
        //});
    }
}
