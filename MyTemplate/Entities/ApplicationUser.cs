using Microsoft.AspNetCore.Identity;

namespace MyTemplate.Entities;

public class ApplicationUser:IdentityUser<Guid>
{
    public ApplicationUser()
    {
        Id= Guid.CreateVersion7();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }

    public string FullName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }= false;
    public DateTime? DisabledDate { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
    