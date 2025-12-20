using MyTemplate.Enums;
using System.Text.Json.Serialization;

namespace MyTemplate.Entities;

public class Permission
{
    public Permission()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; set; }= Guid.NewGuid();

    // Module / Feature
    public string Module { get; set; } = default!;

    // CRUD bits
    public PermissionAction Actions { get; set; }

    [JsonIgnore]
    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}
