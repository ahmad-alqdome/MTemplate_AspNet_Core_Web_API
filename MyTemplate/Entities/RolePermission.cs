using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTemplate.Entities;

public class RolePermission
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Role))]
    public Guid RoleId { get; set; } = default!;
    public ApplicationRole Role { get; set; } = default!;

    [ForeignKey(nameof(Permission))]

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = default!;
}
