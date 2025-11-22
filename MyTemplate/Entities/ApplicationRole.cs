using Microsoft.AspNetCore.Identity;

namespace MyTemplate.Entities;

public class ApplicationRole:IdentityRole<Guid>
{
    public ApplicationRole()
    {
        Id = Guid.CreateVersion7();
    }

    public bool IsDisabled { get; set; } = false;
    public DateTime? DisabledDate { get; set; }

}
