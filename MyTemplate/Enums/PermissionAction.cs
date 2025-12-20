namespace MyTemplate.Enums;

[Flags]
public enum PermissionAction
{
    None = 0,
    Create = 1,
    Read = 2,
    Update = 4,
    Delete = 8
}