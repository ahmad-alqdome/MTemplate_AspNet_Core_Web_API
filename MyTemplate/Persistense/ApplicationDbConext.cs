using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyTemplate.Persistense;

public class ApplicationDbConext:IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
{
    public ApplicationDbConext(DbContextOptions<ApplicationDbConext> options)
     : base(options)
    {
    }
}
